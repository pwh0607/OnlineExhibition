using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
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
    public GameObject cubePrefab;            //ray �浹�� cube
    private Camera cam;
    private GameObject cube;

    private int now_mode;
    void Start()
    {
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        complete.SetActive(false);
        roomManager = GameObject.Find("RoomManager");
        now_mode = 0;
        cube = null;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            m_UI.SetActive(false);
            return;
        }
    
        PlayerMove();
        SetCamPos();
        showRay();
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
        Debug.Log(m_velocity.magnitude);
        transform.LookAt(transform.position + m_velocity);

        m_velocity.y -= gravity * Time.deltaTime;
        controller.Move(m_velocity * m_moveSpeed * Time.deltaTime);

        m_isGrounded = controller.isGrounded;
    }
    public void SetCamPos()
    {
        cam.transform.parent = camPos.transform;
        cam.transform.localPosition = Vector3.zero;
    }

    public void OnclickComplete()
    {
        complete.SetActive(true);
        roomManager.GetComponent<RoomManager>().removeFrame();
        Invoke("DesText", 2.0f);
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
        GameObject instance = null;
        if (now_mode == 0)      //���� ���� ���� ����
        {   
            now_mode = 1;
            instance = Instantiate(cubePrefab);
            cube = instance;
        }
        else if (now_mode == 1)     //�⺻ ���� ����
        {  
            now_mode = 0;
            Destroy(instance);
            cube = null;
        }
    }
    private void showRay()
    {
        if(now_mode == 1)       //���� ���� ����� ���.
        {
            Ray ray = cam.ViewportPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.collider.gameObject.tag == "WALL")   //ray�� ���� �´��� ���.
                {
                    Debug.Log("���� ray�� ��Ҵ�.");
                    Debug.Log("("+Input.mousePosition.x+", " + Input.mousePosition.y+", " + Input.mousePosition.z+")");
                    //cube.SetActive(true);
                    cube.transform.position = hit.point;
                }
                else
                {
                    //cube.SetActive(false);
                }
            }
        }
    }
}