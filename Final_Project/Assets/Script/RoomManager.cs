using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject complete;             //�Ϸ� �ؽ�Ʈ
    public GameObject playerPrefab;
    public GameObject playerPos;
    public GameObject frames;
    private int now_mode;

    public static RoomManager instance // �ܺο��� �̱��� ������Ʈ�� �����ö� ����� ������Ƽ
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<RoomManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }
    private static RoomManager m_instance;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //complete.SetActive(false);
        Vector3 randomSpawnPos = playerPos.transform.position;      //Random.insideUnitSphere * 5f;
        // ��Ʈ��ũ���� ��� Ŭ���̾�Ʈ���� ���� ����  
        // �ش� ���� ������Ʈ�� �ֵ����� ���� �޼��带 ���� ������ Ŭ���̾�Ʈ�� ����
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
        setProperties();
        now_mode = 0;
    }

    public void removeFrame()
    {
        for(int i = 0; i < 5; i++)
        {
            if (frames.transform.GetChild(i).gameObject.GetComponent<FileBrowser>().isImg == false)
            {
                Destroy(frames.transform.GetChild(i).gameObject);       //�̹����� ������ ����.
            }
        }
    }

    public void CompleteEx()
    {
        complete.SetActive(true);
        Invoke("setFalseText", 2.0f);
    }

    private void setFalseText()
    {
        complete.SetActive(false);
    }
    public void LeaveRoom() {
        //���� ������ ���̵�
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("�� ����!");
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.JoinLobby();
        //base.OnLeftRoom();
    }

    public void UpdateMode(int mode)
    {
        this.now_mode = mode;
        Debug.Log("��� ���� : " + now_mode);
    }
    public int GetMode()
    {
        return now_mode;
    }

    //�÷��̾� ���� �ݹ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("�÷��̾� ����!");
    }

    private void setProperties()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "RoomState", "Waiting" } });
        }
    }
}