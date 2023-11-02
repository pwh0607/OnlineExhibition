using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance // �ܺο��� �̱��� ������Ʈ�� ����� �x ������Ƽ
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
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

    private int now_mode;
      //�̱��� ����

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
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
        //setProperties();
        now_mode = 0;
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
}