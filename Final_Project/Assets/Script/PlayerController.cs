using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    public GameObject camPos;
    GameObject roomManager;
    private UIController ui_ctrl;
    private Camera cam;

    //UI
    public GameObject m_UI;
    public GameObject complete;//텍스트
    public GameObject masterPart;
    public GameObject under_UI;   
    public GameObject frames;


    //방 오브젝트 생성용
    public GameObject cube;            //ray 충돌용 cube
    private GameObject colorChanger;

    private int now_mode;
    private bool is_show;       //frame을 보고 있는 상태 인지.

    void Start()
    {
        roomManager = GameObject.Find("RoomManager");
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
        complete.SetActive(false);
        frames = GameObject.Find("Frames");
        ui_ctrl = GetComponent<UIController>();
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

        if (!PhotonNetwork.IsMasterClient)          
        {
            masterPart.SetActive(false);
          //  showFrame();
        }
        else {                                     
            SetImg();
        }

        PlayerMove();
        showCube();
        addFrame();
        SetCamPos();
        SetImg();
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

        //방 공개 여부 open
        PhotonNetwork.CurrentRoom.IsOpen = true;
        
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

    private void showCube()
    {
        if(roomManager.GetComponent<RoomManager>().GetMode() == 1)       //액자 생성 모드인 경우.
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
    //액자 자세히 보기.
    public void showFrame()
    {
        if (Input.GetMouseButtonDown(0))      //방 주인이 아닌 경우.     
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
                    if (roomManager.GetComponent<RoomManager>().GetMode() == 0)      //일반 모드
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

    //액자 이미지 세팅
    public void SetImg()
    {
        if (Input.GetMouseButtonDown(0) && roomManager.GetComponent<RoomManager>().GetMode() == 0)      //기본 모드 이고...
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("액자 클릭!!");
                if (hit.collider.tag == "Frame")
                {
                    Debug.Log("액자 명 : " + hit.collider.gameObject.name);
                    hit.collider.gameObject.GetComponent<FileBrowser>().OnClickImageLoad();
                }
            }
        }   
    }

    //Frame 버튼 관련
    public void OnPointerExit()
    {
        //포인터가 밖으로 나왔을때 모드에 맞도록...
        if (roomManager.GetComponent<RoomManager>().GetMode() == 1)      //
        {
            Debug.Log("모드 : " + roomManager.GetComponent<RoomManager>().GetMode());
            this.cube.SetActive(true);
        }
    }
}