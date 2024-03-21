using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyObj;

    public GameObject nickName;

    public Button RoomBtnPrefab;            //방 입장용 버튼 프리팹
    public GameObject contentView;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    public Text roomName;
    public GameObject emptyRoom;

    private string gameVersion = "1";
    private string makeRoomName;

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        makeRoomName = roomName.GetComponent<Text>().text;
        nickName.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public void CreateRoom()
    {
        //방 옵션 생성
        RoomOptions options = new RoomOptions
        {
            IsOpen = false,
            EmptyRoomTtl = 100000,
            MaxPlayers = 8,
        };

        options.CustomRoomProperties = new Hashtable();
        options.CustomRoomProperties.Add("ownerName", PhotonNetwork.NickName);

        //방 생성하기.
        PhotonNetwork.CreateRoom(makeRoomName, options, null);
    }

    //방 버튼 클릭시!
    public void OnClickRoom(string roomName)
    {
        Debug.Log(roomName + "버튼 클릭!!");
        bool roomJoined = PhotonNetwork.JoinRoom(roomName);

        if (!roomJoined)
        {
            Debug.Log("방이 가득 찼습니다.");
        }
        else
        {
            Debug.Log("방 접속 완료.");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(makeRoomName + "생성 완료!");
    }

    //로비
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접근!");
        cachedRoomList.Clear();
    }

    //방입장
    public override void OnJoinedRoom()
    {
        //해당 방씬으로 이동.
        PhotonNetwork.LoadLevel("RoomScene");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 입장 실패 : " + message);
        base.OnJoinRandomFailed(returnCode, message);
    }

    //방 리스트 가져오기 -- 로비에 접근했을때만 콜백된다!.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("---------------리스트 업데이트-----------------");
        Debug.Log($"방개수 : {roomList.Count}");
        int cen = -40;

        // 방이 열려있는 경우에만 방 버튼 생성
        foreach (var info in roomList){
            int cnt = info.PlayerCount;
            Button tmp;
            if (info.IsOpen)
            {
                tmp = CreateRoomBtn(info.Name, cnt, cen);
                cen += 40;
            }

            if (info.RemovedFromList)
            {
                // 방이 리스트에서 제거된 경우 cachedRoomList에서도 제거
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                // 방이 처음 생성된 경우 cachedRoomList에 추가
                if (!cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }
    }

    Button CreateRoomBtn(string roomName,int playerCnt, int center)
    {
        Button instance = Instantiate(RoomBtnPrefab);           //버튼 instance
        instance.transform.GetChild(0).GetComponent<Text>().text = roomName;
        instance.transform.GetChild(1).GetComponent<Text>().text = playerCnt + " / 8";

        RectTransform btnPos = instance.GetComponent<RectTransform>();
        instance.transform.SetParent(contentView.transform);        //content 뷰의 자식으로 추가

        //버튼 앵커 설정
        Vector2 dir = new Vector2(0.5f, 1f);
        btnPos.anchorMin = dir;
        btnPos.anchorMax = dir;
        btnPos.pivot = dir;

        //설정후 RectTransform 변경.
        btnPos.localPosition = new Vector3(0, 0, 0);
        btnPos.anchoredPosition = new Vector2(0, center);
        btnPos.localScale = new Vector3(1, 1, 1);

        return instance;
}

    //연결 종료
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("방에서 나가 로비로 이동합니다...");
        SceneManager.LoadScene("SignIn");
    }
}