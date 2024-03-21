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

    public Button RoomBtnPrefab;            //�� ����� ��ư ������
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
        //�� �ɼ� ����
        RoomOptions options = new RoomOptions
        {
            IsOpen = false,
            EmptyRoomTtl = 100000,
            MaxPlayers = 8,
        };

        options.CustomRoomProperties = new Hashtable();
        options.CustomRoomProperties.Add("ownerName", PhotonNetwork.NickName);

        //�� �����ϱ�.
        PhotonNetwork.CreateRoom(makeRoomName, options, null);
    }

    //�� ��ư Ŭ����!
    public void OnClickRoom(string roomName)
    {
        Debug.Log(roomName + "��ư Ŭ��!!");
        bool roomJoined = PhotonNetwork.JoinRoom(roomName);

        if (!roomJoined)
        {
            Debug.Log("���� ���� á���ϴ�.");
        }
        else
        {
            Debug.Log("�� ���� �Ϸ�.");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(makeRoomName + "���� �Ϸ�!");
    }

    //�κ�
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ����!");
        cachedRoomList.Clear();
    }

    //������
    public override void OnJoinedRoom()
    {
        //�ش� ������� �̵�.
        PhotonNetwork.LoadLevel("RoomScene");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("�� ���� ���� : " + message);
        base.OnJoinRandomFailed(returnCode, message);
    }

    //�� ����Ʈ �������� -- �κ� ������������ �ݹ�ȴ�!.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("---------------����Ʈ ������Ʈ-----------------");
        Debug.Log($"�氳�� : {roomList.Count}");
        int cen = -40;

        // ���� �����ִ� ��쿡�� �� ��ư ����
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
                // ���� ����Ʈ���� ���ŵ� ��� cachedRoomList������ ����
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                // ���� ó�� ������ ��� cachedRoomList�� �߰�
                if (!cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }
    }

    Button CreateRoomBtn(string roomName,int playerCnt, int center)
    {
        Button instance = Instantiate(RoomBtnPrefab);           //��ư instance
        instance.transform.GetChild(0).GetComponent<Text>().text = roomName;
        instance.transform.GetChild(1).GetComponent<Text>().text = playerCnt + " / 8";

        RectTransform btnPos = instance.GetComponent<RectTransform>();
        instance.transform.SetParent(contentView.transform);        //content ���� �ڽ����� �߰�

        //��ư ��Ŀ ����
        Vector2 dir = new Vector2(0.5f, 1f);
        btnPos.anchorMin = dir;
        btnPos.anchorMax = dir;
        btnPos.pivot = dir;

        //������ RectTransform ����.
        btnPos.localPosition = new Vector3(0, 0, 0);
        btnPos.anchoredPosition = new Vector2(0, center);
        btnPos.localScale = new Vector3(1, 1, 1);

        return instance;
}

    //���� ����
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("�濡�� ���� �κ�� �̵��մϴ�...");
        SceneManager.LoadScene("SignIn");
    }
}