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

    public Button RoomBtnPrefab;           
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
        RoomOptions options = new RoomOptions
        {
            IsOpen = false,
            EmptyRoomTtl = 100000,
            MaxPlayers = 8,
        };

        options.CustomRoomProperties = new Hashtable();
        options.CustomRoomProperties.Add("ownerName", PhotonNetwork.NickName);

        PhotonNetwork.CreateRoom(makeRoomName, options, null);
    }

    public void OnClickRoom(string roomName)
    {
        bool roomJoined = PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    //방 리스트 가져오기 -- 로비에 접근했을때만 콜백된다!.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int cen = -40;

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
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                if (!cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }
    }

    Button CreateRoomBtn(string roomName,int playerCnt, int center)
    {
        Button instance = Instantiate(RoomBtnPrefab);           
        instance.transform.GetChild(0).GetComponent<Text>().text = roomName;
        instance.transform.GetChild(1).GetComponent<Text>().text = playerCnt + " / 8";

        RectTransform btnPos = instance.GetComponent<RectTransform>();
        instance.transform.SetParent(contentView.transform);        

        Vector2 dir = new Vector2(0.5f, 1f);
        btnPos.anchorMin = dir;
        btnPos.anchorMax = dir;
        btnPos.pivot = dir;

        btnPos.localPosition = new Vector3(0, 0, 0);
        btnPos.anchoredPosition = new Vector2(0, center);
        btnPos.localScale = new Vector3(1, 1, 1);

        return instance;
}

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("SignIn");
    }
}