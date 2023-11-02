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

    //�� ������Ʈ ������
    public GameObject cube;            //ray �浹�� cube

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
        //��ư ���� ����
        if (PhotonNetwork.CurrentRoom.IsOpen)
        {
            openBtn_text.GetComponent<Text>().text = "CLOSE";
        }
        else
        {
            //���� ����
            completeText.SetActive(true);
            openBtn_text.GetComponent<Text>().text = "OPEN";
        }
    }

    //��ư ��Ʈ��
    public void LeaveRoom()
    {
        //���� ������ ���̵�
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
            //�̹� �޴� â�� �������¶�� �ݱ�
            menu_clicked = false;
            sidebar.SetActive(false);
        }
        else
        {
            //�޴�â�� ���� ���¶�� ����
            menu_clicked = true;
            sidebar.SetActive(true);
        }
    }

    public void OnClickSetRevise()
    {
        //�� ���� ���� ����.
        revise_part.SetActive(true);
        normal_part.SetActive(false);
    }

    public void OnClickShowCube()
    {
        //ť�� ����.
        RoomManager.instance.UpdateMode(1);
    }

    public void OnClickSetNormal()
    {
        //�� ���� ���� ����.
        normal_part.SetActive(true);
        revise_part.SetActive(false);
        RoomManager.instance.UpdateMode(0);
    }

    private void showCube()
    {
        if (RoomManager.instance.GetMode() == 1)       //���� ���� ����� ���.
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "WALL")   //ray�� ���� �´��� ���.
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
    //Frame ��ư ����
    public void OnPointerEnter()
    {
        this.cube.SetActive(false);
    }

    public void OnPointerExit()
    {
        //�����Ͱ� ������ �������� ��忡 �µ���...
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