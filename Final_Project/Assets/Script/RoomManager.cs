using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
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

    private static RoomManager m_instance; // �̱����� �Ҵ�� static ����
    //public static RoomManager instance;

    public GameObject playerPrefab;
    public GameObject playerPos;

    private int frame_cnt;
    private int now_mode;

    private bool loadChecker;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        //instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {   
        Vector3 SpawnPos = playerPos.transform.position;
        GameObject user =  PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos, Quaternion.identity);
        
        now_mode = 0;
        frame_cnt = 0;
        loadChecker = false;
    }

    public void UpdateMode(int mode)
    {
        this.now_mode = mode;
    }

    public int GetMode()
    {
        return now_mode;
    }

    private void setProperties()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "RoomMaster", PhotonNetwork.LocalPlayer.NickName } });
        }
    }

    public void setOpenOption()
    {
        PhotonNetwork.CurrentRoom.IsOpen = !PhotonNetwork.CurrentRoom.IsOpen;
    }

    public void add_Frame()
    {
        frame_cnt++;
    }
    public int get_FrameCnt()
    {
        return frame_cnt;
    }

    public void loadCheck()
    {
        loadChecker = true;
    }

    public bool checking()
    {
        return loadChecker;
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }


    public RoomManager getInstance()
    {
        return instance;
    }
}