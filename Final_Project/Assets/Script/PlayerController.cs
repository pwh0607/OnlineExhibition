using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using System;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPun
{
    private Animator m_animator;
    private Vector3 m_velocity;                    
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public GameObject m_UI;
    public JoyStick sJoystick;
    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;

    public GameObject camPos;
    private Camera cam;

    private GameObject selectedObj;

    public bool isBtnHover;

    private string roll = "visitor";

    private PlayerModeController PMC;

    void Start()
    {
        isBtnHover = false;
        selectedObj = null;
        PMC = GetComponent<PlayerModeController>();
        
        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();

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
        if(PMC.getMode() == 0)
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
            if (Input.GetMouseButtonDown(0))          
            {
                Debug.Log(hit.collider.gameObject);
                if (hit.collider.gameObject.tag == "Frame")   
                {
                    if (PMC.getMode() == 0)     
                    {
                        selectedObj = hit.collider.gameObject;

                        PMC.setMpde(2);

                        selectedObj.GetComponent<CommentController>().setActionCommentBtn(true);

                        Vector3 targetPosition = selectedObj.transform.position;
                        cam.transform.position = targetPosition - selectedObj.transform.up * 9;
                        cam.transform.LookAt(selectedObj.transform);
                    }
                }
                else{
                    if(selectedObj != null)
                    {
                        if(!selectedObj.GetComponent<CommentController>().commentBtn.GetComponent<FrameUICheckSC>().isHovering && !selectedObj.GetComponent<CommentController>().commentPart.GetComponent<FrameUICheckSC>().isHovering)
                        {
                            selectedObj.GetComponent<CommentController>().setActionCommentBtn(false);
                            PMC.setMpde(0);
                            selectedObj = null;
                        }
                    }
                }
            }
        }
    }

    public void SetImg()
    {
        if (Input.GetMouseButtonDown(0) && PMC.getMode() == 0)      
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