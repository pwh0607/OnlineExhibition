using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIController : MonoBehaviourPunCallbacks
{
    private PlayerController player_ctrl;

    public GameObject camPos;
    public GameObject roomManager;
    public UIController ui_ctrl;
    private Camera cam;

    //UI
    public GameObject m_UI;
    public GameObject complete;//�ؽ�Ʈ
    public GameObject masterPart;
    public GameObject under_UI;
    public GameObject frames;

    //�� ������Ʈ ������
    public GameObject cube;            //ray �浹�� cube
    private GameObject colorChanger;

    private int now_mode;
    private bool is_show;       //frame�� ���� �ִ� ���� ����.

    void Start()
    {
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        complete.SetActive(false);
        roomManager = GameObject.Find("RoomManager");
        frames = GameObject.Find("Frames");
        ui_ctrl = GetComponent<UIController>();
        is_show = false;
        cube.SetActive(false);
        player_ctrl = GetComponent<PlayerController>();
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
        SetCamPos();
        SetImg();
    }
    public void SetCamPos()
    {
        if (now_mode == 0)
        {
            cam.transform.parent = camPos.transform;
            cam.transform.localPosition = Vector3.zero;
        }
    }

    public void OnclickComplete()
    {
        complete.SetActive(true);
        roomManager.GetComponent<RoomManager>().removeFrame();
        Invoke("DesText", 2.0f);

        //�� ���� ���� open
        PhotonNetwork.CurrentRoom.IsOpen = true;
    }
    public void DesText()
    {
        complete.SetActive(false);
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
    public void setMode()
    {
        if (roomManager.GetComponent<RoomManager>().GetMode() == 0)      //���� ���� ���� ����
        {
            roomManager.GetComponent<RoomManager>().UpdateMode(1);
            cube.SetActive(true);
        }
        else if (roomManager.GetComponent<RoomManager>().GetMode() == 1)     //�⺻ ���� ����
        {
            roomManager.GetComponent<RoomManager>().UpdateMode(0);
            Debug.Log("�⺻ ���� ����");
            cube.SetActive(false);
        }
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

    public void OnClickShowBtn()
    {
        under_UI.SetActive(!under_UI.activeSelf);
    }

    private void addFrame()
    {
        if (Input.GetMouseButtonDown(0) && cube.activeSelf)
        {
            cube.GetComponent<ColorChanger>().AddFrame();
        }
    }
    //���� �ڼ��� ����.
    public void showFrame()
    {
        if (Input.GetMouseButtonDown(0))      //�� ������ �ƴ� ���.     
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Frame")   //ray�� ���ڿ� �´��� ����.
                {
                    is_show = true;
                    now_mode = 2;
                    Debug.Log("���ڿ� ray�� ��Ҵ�.");
                    if (roomManager.GetComponent<RoomManager>().GetMode() == 0)      //�Ϲ� ���
                    {
                        //ī�޶� ��ġ�� ������ z���� ...�����. z -> -9.3f
                        Vector3 hitObj = hit.collider.gameObject.transform.position;
                        Vector3 camPos = new Vector3(hitObj.x, hitObj.y, hitObj.z - 9.3f);
                        cam.transform.position = camPos;
                    }
                }
            }
        }
    }

    //���� �̹��� ����
    public void SetImg()
    {
        if (Input.GetMouseButtonDown(0) && roomManager.GetComponent<RoomManager>().GetMode() == 0)      //�⺻ ��� �̰�...
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("���� Ŭ��!!");
                if (hit.collider.tag == "Frame")
                {
                    Debug.Log("���� �� : " + hit.collider.gameObject.name);
                    hit.collider.gameObject.GetComponent<FileBrowser>().OnClickImageLoad();
                }
            }
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
        if (roomManager.GetComponent<RoomManager>().GetMode() == 1)      //
        {
            Debug.Log("��� : " + roomManager.GetComponent<RoomManager>().GetMode());
            this.cube.SetActive(true);
        }
    }
}
