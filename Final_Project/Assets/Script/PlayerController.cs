using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    //움직임
    private Animator m_animator;
    private Vector3 m_velocity;                     //이동방향
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public JoyStick sJoystick;
    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;

    //UI
    public GameObject m_UI;
    public GameObject camPos;
    public GameObject complete;         //텍스트
    public GameObject roomManager;
    public GameObject cubePrefab;            //ray 충돌용 cube
    private Camera cam;
    
    //방 오브젝트 생성용
    private GameObject cube;
    private GameObject colorChanger;

    private int now_mode;
    private bool is_show;       //frame을 보고 있는 상태 인지.
    void Start()
    {
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        complete.SetActive(false);
        roomManager = GameObject.Find("RoomManager");
        now_mode = 0;
        cube = null;
        is_show = false;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            m_UI.SetActive(false);
            return;
        }
    
        PlayerMove();
        showRay();
      //  addFrame();
        showFrame();
        SetCamPos();
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
        GameObject instance = null;
        if (roomManager.GetComponent<RoomManager>().GetMode() == 0)      //액자 생성 모드로 변경
        {   
            roomManager.GetComponent<RoomManager>().UpdateMode(1);
            if (cube == null)    //큐브가 null인 경우.
            {
                instance = Instantiate(cubePrefab);
                cube = instance;
            }
            else
            {
                cube.SetActive(true);
            }
        }
        else if (roomManager.GetComponent<RoomManager>().GetMode() == 1)     //기본 모드로 변경
        {
            roomManager.GetComponent<RoomManager>().UpdateMode(0);
            Debug.Log("기본 모드로 변경");
            cube.SetActive(false);
        }
    }
    private void showRay()
    {
        if(roomManager.GetComponent<RoomManager>().GetMode() == 1)       //액자 생성 모드인 경우.
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "WALL")   //ray가 벽에 맞닿은 경우.
                {
                    Debug.Log("벽에 ray가 닿았다.");
                    cube.transform.position = hit.point;
                    cube.transform.rotation = hit.collider.gameObject.transform.rotation;
                }
            }   
        }
    }
    private void addFrame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("액자 생성11111111111");
            if(cube != null || cube.activeSelf)         //큐브가 없거나 활성화 되어있는 경우.
            {
                //cube.GetComponent<ColorChanger>().AddFrame();
            }
        }
    }
    //액자 자세히 보기.
    public void showFrame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Frame")   //ray가 액자에 맞닿은 순간.
                {
                    is_show = true;
                    now_mode = 2;
                    Debug.Log("액자에 ray가 닿았다.");
                    if (roomManager.GetComponent<RoomManager>().GetMode() == 0) //일반 모드
                    {
                        //카메라 위치를 액자의 z값에 ...블라블라. z -> -9.3f
                        Vector3 hitObj = hit.collider.gameObject.transform.position;
                        Vector3 camPos = new Vector3(hitObj.x, hitObj.y, hitObj.z - 9.3f);
                        cam.transform.position = camPos;
                    }
                }
            }
        }
    }
}