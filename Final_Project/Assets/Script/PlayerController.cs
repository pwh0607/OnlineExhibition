using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    //������
    private Animator m_animator;
    private Vector3 m_velocity;                     //�̵�����
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public JoyStick sJoystick;
    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;

    //UI
    public GameObject m_UI;
    public GameObject camPos;
    public GameObject complete;         //�ؽ�Ʈ
    public GameObject roomManager;
    public GameObject masterPart;
    public GameObject under_UI;

    private Camera cam;

    //�� ������Ʈ ������
    public GameObject cube;            //ray �浹�� cube
    private GameObject colorChanger;

    private int now_mode;
    private bool is_show;       //frame�� ���� �ִ� ���� ����.

    void Start()
    {
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        complete.SetActive(false);
        roomManager = GameObject.Find("RoomManager");
        is_show = false;
        cube.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            m_UI.SetActive(false);
            return;
        }

        if (!PhotonNetwork.IsMasterClient)           //�������� ���.
        {
            masterPart.SetActive(false);
        }
        
        PlayerMove();
        showRay();
        addFrame();
        showFrame();
        SetCamPos();
    }
    private void PlayerMove()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float gravity = 20.0f;              //���� �߷��� �־�

        float h = sJoystick.GetHorizontalValue();
        float v = sJoystick.GetVerticalValue();
        m_velocity = new Vector3(h, 0, v);
        m_velocity = m_velocity.normalized;

        m_animator.SetFloat("Move", m_velocity.magnitude);
        transform.LookAt(transform.position + m_velocity);

        m_velocity.y -= gravity * Time.deltaTime;
        controller.Move(m_velocity * m_moveSpeed * Time.deltaTime);

        m_isGrounded = controller.isGrounded;
    }
    public void SetCamPos()
    {
        if(now_mode == 0)
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
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "RoomState", "Waiting" } });
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
    private void showRay()
    {
        if(roomManager.GetComponent<RoomManager>().GetMode() == 1)       //���� ���� ����� ���.
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "WALL")   //ray�� ���� �´��� ���.
                {
                    Debug.Log("���� ray�� ��Ҵ�.");
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
        if (Input.GetMouseButtonDown(0))
        {
            cube.GetComponent<ColorChanger>().AddFrame();
        }
    }
    //���� �ڼ��� ����.
    public void showFrame()
    {
        if (Input.GetMouseButtonDown(0) && !PhotonNetwork.IsMasterClient)      //�� ������ �ƴ� ���.     
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
}