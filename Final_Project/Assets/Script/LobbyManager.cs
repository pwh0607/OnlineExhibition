using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

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
        nickName.GetComponent<Text>().text = PhotonNetwork.NickName;
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public void CreateRoom()
    {
        //방 옵션 생성
        RoomOptions options = new RoomOptions();
        options.IsOpen = false;

        //방 생성하기.
        PhotonNetwork.CreateRoom(makeRoomName, options, null);
    }

    //방 버튼 클릭시!
    public void OnClickRoom(string roomName)
    {
        Debug.Log(roomName + "버튼 클릭!!");
        PhotonNetwork.JoinRandomRoom();
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
    public override void OnLeftLobby()
    {

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
        //리스트를 가져와서 버튼형식으로 생성!
        //cachedRoomList.Clear();
        Debug.Log("---------------리스트 업데이트-----------------");
        Debug.Log($"방개수 : {roomList.Count}");

        for (int i = 0; i < roomList.Count; i++)
        {
            //방의 상태가 Open인 경우만...
            // if (roomList[i].IsOpen)
            {
                Button instance = Instantiate(RoomBtnPrefab);           //버튼 instance
                instance.transform.GetChild(0).GetComponent<Text>().text = roomList[i].Name;
                RectTransform btnPos = instance.GetComponent<RectTransform>();
                instance.transform.SetParent(contentView.transform);        //content 뷰의 자식으로 추가

                //버튼 앵커 설정
                Vector2 dir = new Vector2(0.5f, 1);
                btnPos.anchorMin = dir;
                btnPos.anchorMax = dir;
                btnPos.pivot = dir;

                //설정후 RectTransform 변경.
                btnPos.anchoredPosition = new Vector2(0, -40);
            }
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList == false)
            {
                if (cachedRoomList.ContainsKey(roomList[i].Name) == false)      //방이 처음 생성된 경우.
                {
                    cachedRoomList.Add(roomList[i].Name, roomList[i]);
                }
            }
        }
    }
    //연결 종료
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("SignIn");
    }
}