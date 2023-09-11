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
    public GameObject complete;             //완료 텍스트
    public GameObject playerPrefab;
    public GameObject playerPos;
    public GameObject frames;
    private int now_mode;

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
    private static RoomManager m_instance;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //complete.SetActive(false);
        Vector3 randomSpawnPos = playerPos.transform.position;      //Random.insideUnitSphere * 5f;
        // 네트워크상의 모든 클라이언트에서 생성 실행  
        // 해당 게임 오브젝트의 주도권은 생성 메서드를 직접 실행한 클라이언트에 있음
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
                Destroy(frames.transform.GetChild(i).gameObject);       //이미지가 없으면 삭제.
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
        //방을 나오고 씬이동
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장!");
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.JoinLobby();
        //base.OnLeftRoom();
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

    //플레이어 입장 콜백
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("플레이어 입장!");
    }

    private void setProperties()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "RoomState", "Waiting" } });
        }
    }
}