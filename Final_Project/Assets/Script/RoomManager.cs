using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance 
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<RoomManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static RoomManager m_instance;

    public GameObject playerPrefab;
    public GameObject playerPos;

    private int frame_cnt;
    private int now_mode;

    private bool loadChecker;
      //�̱��� ����

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("�÷��̾� ���� !");
        Vector3 SpawnPos = playerPos.transform.position;
        PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos, Quaternion.identity);
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

    public override void OnLeftRoom()
    {
        Debug.Log("�� ������");
        PhotonNetwork.LoadLevel("LobbyScene");
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
}