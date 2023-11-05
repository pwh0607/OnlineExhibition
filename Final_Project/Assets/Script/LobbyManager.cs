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
        nickName.GetComponent<Text>().text = PhotonNetwork.NickName;
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public void CreateRoom()
    {
        //�� �ɼ� ����
        RoomOptions options = new RoomOptions();
        options.IsOpen = false;

        //�� �����ϱ�.
        PhotonNetwork.CreateRoom(makeRoomName, options, null);
    }

    //�� ��ư Ŭ����!
    public void OnClickRoom(string roomName)
    {
        Debug.Log(roomName + "��ư Ŭ��!!");
        PhotonNetwork.JoinRandomRoom();
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
    public override void OnLeftLobby()
    {

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
        //����Ʈ�� �����ͼ� ��ư�������� ����!
        //cachedRoomList.Clear();
        Debug.Log("---------------����Ʈ ������Ʈ-----------------");
        Debug.Log($"�氳�� : {roomList.Count}");

        for (int i = 0; i < roomList.Count; i++)
        {
            //���� ���°� Open�� ��츸...
            // if (roomList[i].IsOpen)
            {
                Button instance = Instantiate(RoomBtnPrefab);           //��ư instance
                instance.transform.GetChild(0).GetComponent<Text>().text = roomList[i].Name;
                RectTransform btnPos = instance.GetComponent<RectTransform>();
                instance.transform.SetParent(contentView.transform);        //content ���� �ڽ����� �߰�

                //��ư ��Ŀ ����
                Vector2 dir = new Vector2(0.5f, 1);
                btnPos.anchorMin = dir;
                btnPos.anchorMax = dir;
                btnPos.pivot = dir;

                //������ RectTransform ����.
                btnPos.anchoredPosition = new Vector2(0, -40);
            }
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList == false)
            {
                if (cachedRoomList.ContainsKey(roomList[i].Name) == false)      //���� ó�� ������ ���.
                {
                    cachedRoomList.Add(roomList[i].Name, roomList[i]);
                }
            }
        }
    }
    //���� ����
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("SignIn");
    }
}