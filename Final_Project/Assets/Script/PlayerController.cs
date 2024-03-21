using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;
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
    private GameObject selectedObj;

    public bool isBtnHover;

    private string roll = "visitor";

    void Start()
    {
        isBtnHover = false;
        selectedObj = null;
        now_mode = 0;
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        
        m_animator = GetComponent<Animator>();
        try
        {
            cam = GameObject.Find("mainCam").GetComponent<Camera>();

        }catch(Exception e)
        {
            Debug.Log("카메라 인식 실패");
        }

        //방주인 이름 가져오기용
        Hashtable hashRef = PhotonNetwork.CurrentRoom.CustomProperties;
        
        if((string)hashRef["ownerName"] == PhotonNetwork.NickName)
        {
            roll = "owner";
        }
        else
        {
            roll = "visitor";
        }
    }
    void Update()
    {
        
        if (!photonView.IsMine)
        {
            m_UI.SetActive(false);
            return;
        }
        
        if(roll == "visitor")
        {
            showFrame();
        }
        else
        {
            SetImg();
        }

        PlayerMove();
        SetCamPos();
    }
    private void PlayerMove()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float gravity = 20.0f;             

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
            cam.transform.position = camPos.transform.position;
            cam.transform.rotation = camPos.transform.rotation;
        }
    }
    public void showFrame()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (Input.GetMouseButtonDown(0))      //방 주인이 아닌 경우.     
            {
                Debug.Log(hit.collider.gameObject);
                if (hit.collider.gameObject.tag == "Frame")   //ray가 액자에 맞닿은 순간.
                {
                    if (RoomManager.instance.GetMode() == 0)      //일반 모드
                    {
                        selectedObj = hit.collider.gameObject;

                        //2번은 액자 보기 모드.
                        now_mode = 2;

                        //댓글 버튼 활성화.
                        selectedObj.GetComponent<CommentController>().setActionCommentBtn(true);

                        Vector3 targetPosition = selectedObj.transform.position;

                        cam.transform.position = targetPosition - selectedObj.transform.up * 9;
                        cam.transform.LookAt(selectedObj.transform);
                    }
                }
                else{
                    //버튼 클릭시 해당 함수 무시.
                    if(selectedObj != null)
                    {
                        if(!selectedObj.GetComponent<CommentController>().commentBtn.GetComponent<FrameUICheckSC>().isHovering && !selectedObj.GetComponent<CommentController>().commentPart.GetComponent<FrameUICheckSC>().isHovering)
                        {
                            selectedObj.GetComponent<CommentController>().setActionCommentBtn(false);
                            now_mode = 0;
                            selectedObj = null;
                        }
                    }
                }
                Debug.Log("select : " + selectedObj);
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
    public Camera getCamRef()
    {
        return cam;
    }
}