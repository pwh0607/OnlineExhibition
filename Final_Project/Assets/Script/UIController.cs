using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIController : MonoBehaviourPunCallbacks
{
    private PlayerController player_ctrl;

    private Camera cam;

    //UI
    public GameObject m_UI;
    public GameObject completeText;
    public GameObject openBtn_text;

    public GameObject master_menu;
    public GameObject customer_menu;

    //방 오브젝트 생성용
    public GameObject cube;            //ray 충돌용 cube

    private bool menu_clicked = false;


    public GameObject master_sidebar;
    public GameObject customer_sidebar;

    private GameObject sidebar;

    public GameObject revise_part;
    public GameObject normal_part;
   
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            sidebar = customer_sidebar;
            customer_menu.SetActive(true);
            master_menu.SetActive(false);
        }
        else
        {
            sidebar = master_sidebar;
            master_menu.SetActive(true);
            customer_menu.SetActive(false);
        }
        
        sidebar = master_sidebar;
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        cube.SetActive(false);
        player_ctrl = GetComponent<PlayerController>();
        sidebar.SetActive(false);
        revise_part.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            m_UI.SetActive(false);
            return;
        }

        showCube();
        addFrame();
    }
    public void OnClickOpen()
    {
        RoomManager.instance.setOpenOption();
        //버튼 상태 변경
        if (PhotonNetwork.CurrentRoom.IsOpen)
        {
            openBtn_text.GetComponent<Text>().text = "CLOSE";
        }
        else
        {
            //문구 띄우기
            completeText.SetActive(true);
            openBtn_text.GetComponent<Text>().text = "OPEN";
        }
    }

    //버튼 컨트롤
    public void LeaveRoom()
    {
        //방을 나오고 씬이동
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void OnClickMenuBtn()
    {
        if (menu_clicked)
        {
            //이미 메뉴 창이 열린상태라면 닫기
            menu_clicked = false;
            sidebar.SetActive(false);
        }
        else
        {
            //메뉴창이 닫힌 상태라면 열기
            menu_clicked = true;
            sidebar.SetActive(true);
        }
    }

    public void OnClickSetRevise()
    {
        //방 수정 모드로 변경.
        revise_part.SetActive(true);
        normal_part.SetActive(false);
    }

    public void OnClickShowCube()
    {
        //큐브 생성.
        RoomManager.instance.UpdateMode(1);
    }

    public void OnClickSetNormal()
    {
        //방 수정 모드로 변경.
        normal_part.SetActive(true);
        revise_part.SetActive(false);
        RoomManager.instance.UpdateMode(0);
    }

    private void showCube()
    {
        if (RoomManager.instance.GetMode() == 1)       //액자 생성 모드인 경우.
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
        if (RoomManager.instance.GetMode() == 1) 
        {
            this.cube.SetActive(true);
        }
    }
    public void showText()
    {
        completeText.SetActive(true);
    }
}