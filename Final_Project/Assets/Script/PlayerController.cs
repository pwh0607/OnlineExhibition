using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private Animator m_animator;
    private Vector3 m_velocity;                     //이동방향
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public JoyStick sJoystick;
    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;

    //UI
    public GameObject m_UI;
    public Camera cam;
    public GameObject camPos;
    public GameObject complete;         //텍스트
    public GameObject roomManager;
    public GameObject cube;             //ray 충돌용 cube
    private int now_mode;
    void Start()
    {
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        complete.SetActive(false);
        roomManager = GameObject.Find("RoomManager");
        now_mode = 0;
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
        float gravity = 20.0f;              //강한 중력을 주어

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
    public void setMode()
    {
        if (now_mode == 0)   //액자 생성 모드로 변경
            now_mode = 1;
        else if (now_mode == 1)  //기본 모드로 변경
            now_mode = 0;
    }
    private void showRay()
    {
        if(now_mode == 1)       //액자 생성 모드인 경우.
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.collider.gameObject.tag == "WALL")   //ray가 벽에 맞닿은 경우.
                {
                    Debug.Log("벽에 ray가 닿았다.");
                    cube.SetActive(true);
                    cube.transform.position = hit.point;
                }
                else
                {
                    cube.SetActive(false);
                }
            }
        }
    }
}