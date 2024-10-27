using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

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
            return m_instance;
        }
    }

    private static RoomManager m_instance;
    static List<string> users;

    public GameObject playerPrefab;
    public GameObject playerPos;

    private int frame_cnt;

    private bool loadChecker;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {   
        Vector3 SpawnPos = playerPos.transform.position;
        GameObject user =  PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos, Quaternion.identity);
        frame_cnt = 0;
        loadChecker = false;
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
    public void setUser(string userName)
    {
        users.Add(userName);
    }

    public RoomManager getInstance()
    {
        return instance;
    }
}