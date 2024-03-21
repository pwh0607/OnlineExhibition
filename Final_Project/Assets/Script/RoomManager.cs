using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<RoomManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static RoomManager m_instance; // 싱글톤이 할당될 static 변수
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