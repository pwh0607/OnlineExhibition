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

    private PlayerModeController PMC;

    public GameObject m_UI;
    public GameObject completeText;
    public GameObject openBtn_text;

    public GameObject master_menu;
    public GameObject customer_menu;

    public GameObject ReviseMV;
    public GameObject DeleteMV;

    public GameObject cube;           

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
        
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        PMC = GetComponent<PlayerModeController>();

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

        if (PhotonNetwork.CurrentRoom.IsOpen)           
        {
            completeText.SetActive(true);
            openBtn_text.GetComponent<TextMeshProUGUI>().text = "OPEN";
        }
        else
        {
            openBtn_text.GetComponent<TextMeshProUGUI>().text = "CLOSE";
        }
    }

    public void LeaveRoom()
    {
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

        PhotonNetwork.LeaveRoom();
    }

    public void OnClickMenuBtn()
    {
        if (menu_clicked)
        {
            menu_clicked = false;
            sidebar.SetActive(false);
        }
        else
        {
            menu_clicked = true;
            sidebar.SetActive(true);
        }
    }

    public void OnClickSetRevise()
    {
        revise_part.SetActive(true);
        normal_part.SetActive(false);
    }

    public void OnClickShowCube()
    {
        PMC.setMpde(1);
        ReviseMV.SetActive(true);
    }

    public void OnClickSetNormal()
    {
        normal_part.SetActive(true);
        revise_part.SetActive(false);

        ReviseMV.SetActive(false);
        DeleteMV.SetActive(false);
        PMC.setMpde(0);
    }

    private void showCube()
    {
        if (PMC.getMode() == 1)       
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "WALL")   
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

    public void OnPointerEnter()
    {
        this.cube.SetActive(false);
    }

    public void OnPointerExit()
    {
        if (PMC.getMode() == 1) 
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
        if(PMC.getMode() == 3)
        {
            PMC.setMpde(0);
            DeleteMV.SetActive(false);
        }
        else
        {
            PMC.setMpde(3);
            DeleteMV.SetActive(true);
        }
    }
    
    public void deleteFrame()
    {
        if (PMC.getMode() == 3 && Input.GetMouseButtonDown(0))
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
}