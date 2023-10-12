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

    //�� ������Ʈ ������
    public GameObject cube;            //ray �浹�� cube

    private bool menu_clicked = false;

    public GameObject menu_sidebar;
    public GameObject revise_part;
    public GameObject normal_part;

    private int now_mode;
    private bool is_show;       //frame�� ���� �ִ� ���� ����.

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
        Debug.Log("���� �Ϸ�!");
        Debug.Log("�� ���� ���� : " + PhotonNetwork.CurrentRoom.IsOpen);

        //���� ����
        Instantiate(completeText);
    }

    //��ư ��Ʈ��
    public void LeaveRoom()
    {
        //���� ������ ���̵�
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("�� ����!");
        PhotonNetwork.LoadLevel("Lobby");
        PhotonNetwork.JoinLobby();
    }

    public void OnClickMenuBtn()
    {
        if (menu_clicked)
        {
            //�̹� �޴� â�� �������¶�� �ݱ�
            menu_clicked = false;
            menu_sidebar.SetActive(false);
        }
        else
        {
            //�޴�â�� ���� ���¶�� ����
            menu_clicked = true;
            menu_sidebar.SetActive(true);
        }
    }

    public void OnClickSetRevise()
    {
        //�� ���� ���� ����.
        Debug.Log("��� : " + roomManager.GetComponent<RoomManager>().GetMode());
        revise_part.SetActive(true);
        normal_part.SetActive(false);
    }

    public void OnClickShowCube()
    {
        //ť�� ����.
        roomManager.GetComponent<RoomManager>().UpdateMode(1);
    }

    public void OnClickSetNormal()
    {
        //�� ���� ���� ����.
        Debug.Log("��� : " + roomManager.GetComponent<RoomManager>().GetMode());
        normal_part.SetActive(true);
        revise_part.SetActive(false);
        roomManager.GetComponent<RoomManager>().UpdateMode(0);
    }
    private void showCube()
    {
        if (roomManager.GetComponent<RoomManager>().GetMode() == 1)       //���� ���� ����� ���.
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
        if (roomManager.GetComponent<RoomManager>().GetMode() == 1) 
        {
            Debug.Log("��� : " + roomManager.GetComponent<RoomManager>().GetMode());
            this.cube.SetActive(true);
        }
    }
    public void showText()
    {
        completeText.SetActive(true);
    }
}