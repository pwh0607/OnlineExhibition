using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance // 외부에서 싱글톤 오브젝트를 사용할 땨 프로퍼티
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<RoomManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static RoomManager m_instance;

    public GameObject playerPrefab;
    public GameObject playerPos;

    private int now_mode;
      //싱글톤 변수

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("플레이어 생성 !");
        Vector3 SpawnPos = playerPos.transform.position;
        PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos, Quaternion.identity);
        //setProperties();
        now_mode = 0;
    }

    public void UpdateMode(int mode)
    {
        this.now_mode = mode;
        Debug.Log("모드 변경 : " + now_mode);
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