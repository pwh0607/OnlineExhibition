using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    //움직임
    private Animator m_animator;
    private Vector3 m_velocity;                     //이동방향
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public GameObject m_UI;
    public JoyStick sJoystick;
    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;

    public GameObject camPos;
    private Camera cam;

   
    private int now_mode;
   
    void Start()
    {
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
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
                    now_mode = 2;
                    if (RoomManager.instance.GetMode() == 0)      //일반 모드
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
        if (Input.GetMouseButtonDown(0) && RoomManager.instance.GetMode() == 0)      //기본 모드 이고...
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Frame")
                {
                    hit.collider.gameObject.GetComponent<FBImgUploader>().OnClickImageLoad();
                }
            }
        }   
    }
}