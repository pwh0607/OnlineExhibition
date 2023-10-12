using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIController : MonoBehaviourPunCallbacks
{
    private PlayerController player_ctrl;
    
    public GameObject roomManager;
    private Camera cam;

    //UI
    public GameObject m_UI;
    public GameObject masterPart;
    public GameObject completeText;

    //방 오브젝트 생성용
    public GameObject cube;            //ray 충돌용 cube

    private bool menu_clicked = false;

    public GameObject menu_sidebar;
    public GameObject revise_part;
    public GameObject normal_part;

    private int now_mode;
    private bool is_show;       //frame을 보고 있는 상태 인지.

    void Start()
    {
        roomManager = GameObject.Find("RoomManager");
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        is_show = false;
        cube.SetActive(false);
        player_ctrl = GetComponent<PlayerController>();
        menu_sidebar.SetActive(false);
        revise_part.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            m_UI.SetActive(false);
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            masterPart.SetActive(false);
        }

        showCube();
        addFrame();
    }
    public void OnClickOpen()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        Debug.Log("오픈 완료!");
        Debug.Log("방 오픈 상태 : " + PhotonNetwork.CurrentRoom.IsOpen);

        //문구 띄우기
        Instantiate(completeText);
    }

    //버튼 컨트롤
    public void LeaveRoom()
    {
        //방을 나오고 씬이동
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장!");
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.JoinLobby();
    }

    public void OnClickMenuBtn()
    {
        if (menu_clicked)
        {
            //이미 메뉴 창이 열린상태라면 닫기
            menu_clicked = false;
            menu_sidebar.SetActive(false);
        }
        else
        {
            //메뉴창이 닫힌 상태라면 열기
            menu_clicked = true;
            menu_sidebar.SetActive(true);
        }
    }

    public void OnClickSetRevise()
    {
        //방 수정 모드로 변경.
        Debug.Log("모드 : " + roomManager.GetComponent<RoomManager>().GetMode());
        revise_part.SetActive(true);
        normal_part.SetActive(false);
    }

    public void OnClickShowCube()
    {
        //큐브 생성.
        roomManager.GetComponent<RoomManager>().UpdateMode(1);
    }

    public void OnClickSetNormal()
    {
        //방 수정 모드로 변경.
        Debug.Log("모드 : " + roomManager.GetComponent<RoomManager>().GetMode());
        normal_part.SetActive(true);
        revise_part.SetActive(false);
        roomManager.GetComponent<RoomManager>().UpdateMode(0);
    }
    private void showCube()
    {
        if (roomManager.GetComponent<RoomManager>().GetMode() == 1)       //액자 생성 모드인 경우.
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "WALL")   //ray가 벽에 맞닿은 경우.
                {
                    cube.transform.position = hit.point;
                    cube.transform.rotation = hit.collider.gameObject.transform.rotation;
                }
            }
        }
    }

    private void addFrame()
    {
        if (Input.GetMouseButtonDown(0) && cube.activeSelf)
        {
            cube.GetComponent<ColorChanger>().AddFrame();
        }
    }
    //Frame 버튼 관련
    public void OnPointerEnter()
    {
        this.cube.SetActive(false);
    }

    public void OnPointerExit()
    {
        //포인터가 밖으로 나왔을때 모드에 맞도록...
        if (roomManager.GetComponent<RoomManager>().GetMode() == 1) 
        {
            Debug.Log("모드 : " + roomManager.GetComponent<RoomManager>().GetMode());
            this.cube.SetActive(true);
        }
    }
    public void showText()
    {
        completeText.SetActive(true);
    }
}