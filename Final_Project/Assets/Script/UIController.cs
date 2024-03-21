using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;

public class UIController : MonoBehaviourPunCallbacks
{
    private Camera cam;

    //UI
    public GameObject m_UI;
    public GameObject completeText;
    public GameObject openBtn_text;

    public GameObject master_menu;
    public GameObject customer_menu;

    public GameObject ReviseMV;
    public GameObject DeleteMV;

    //�� ������Ʈ ������
    public GameObject cube;            //���� ������ ť��

    private bool menu_clicked = false;

    public GameObject master_sidebar;
    public GameObject customer_sidebar;

    private GameObject sidebar;

    public GameObject revise_part;
    public GameObject normal_part;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)          //�� ������ �ƴ� ���.
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
        
        //sidebar = master_sidebar;
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        cube.SetActive(false);
        sidebar.SetActive(false);
        revise_part.SetActive(false);
        ReviseMV.SetActive(false);
        DeleteMV.SetActive(false);
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
        deleteFrame();
    }

    public void OnClickOpen()
    {
        RoomManager.instance.setOpenOption();
        //��ư ���� ����
        if (PhotonNetwork.CurrentRoom.IsOpen)           //���� ���¶��.
        {
            //���� ����
            completeText.SetActive(true);
            openBtn_text.GetComponent<TextMeshProUGUI>().text = "OPEN";
        }
        else
        {
            openBtn_text.GetComponent<TextMeshProUGUI>().text = "CLOSE";
        }
    }

    //��ư ��Ʈ��s
    public void LeaveRoom()
    {

        Debug.Log("�� ������");
        PhotonNetwork.LeaveRoom(); 
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void DeleteRoom()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;

        //�� ������ ���ε� ����.
        PhotonNetwork.LeaveRoom();
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
        ReviseMV.SetActive(true);
    }

    //�Ϸ� ��ư�� ������ ��...
    public void OnClickSetNormal()
    {
        //�� ���� ���� ����.
        normal_part.SetActive(true);
        revise_part.SetActive(false);

        //����� ��Ȱ��ȭ.
        ReviseMV.SetActive(false);
        DeleteMV.SetActive(false);
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
        if (RoomManager.instance.GetMode() == 1) 
        {
            this.cube.SetActive(true);
        }
    }
    public void showText()
    {
        completeText.SetActive(true);
    }

    public void setDeleteMode()
    {
        if(RoomManager.instance.GetMode() == 3)
        {
            RoomManager.instance.UpdateMode(0);
            DeleteMV.SetActive(false);
            Debug.Log("�Ϲ� ���� ����.");
        }
        else
        {
            RoomManager.instance.UpdateMode(3);
            DeleteMV.SetActive(true);
            Debug.Log("���� ���� ����.");
        }
    }
    
    public void deleteFrame()
    {
        if (RoomManager.instance.GetMode() == 3 && Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Frame")
                    Destroy(hit.collider.gameObject);
            }
        }
    }

    public void showCommentBtn()
    {
       
    }
}