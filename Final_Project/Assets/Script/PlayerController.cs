using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerController : MonoBehaviourPun
{
    //������
    private Animator m_animator;
    private Vector3 m_velocity;                     //�̵�����
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public GameObject m_UI;
    public JoyStick sJoystick;
    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;

    public GameObject camPos;
    private Camera cam;

    private int now_mode;
    private string roll = "visitor";

    void Start()
    {
        if (!photonView.IsMine)
            gameObject.SetActive(false);

        m_animator = GetComponent<Animator>();
        cam = GameObject.Find("mainCam").GetComponent<Camera>();

        //������ �̸� ���������
        Hashtable hashRef = PhotonNetwork.CurrentRoom.CustomProperties;
        Debug.Log("������ : " + hashRef["ownerName"]);

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
            //�湮���� ���� �ڼ��� ���� ��� �߰�.
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

    //���� �ڼ��� ���� - �湮�� ����.
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
                    if (RoomManager.instance.GetMode() == 0)      //�Ϲ� ���
                    {
                        GameObject photo = hit.collider.gameObject;
                        now_mode = 2;
                        Vector3 hitObj = photo.transform.position;
                        Vector3 camPos = new Vector3(hitObj.x, hitObj.y + 2, hitObj.z - 8.0f);

                        cam.transform.position = camPos;
                        cam.transform.rotation = Quaternion.identity;
                    }
                }
                else{
                    //���� ��Ŭ����.
                    now_mode = 0;
                }
            }
        }
    }

    //���� �̹��� ����
    public void SetImg()
    {
        if (Input.GetMouseButtonDown(0) && RoomManager.instance.GetMode() == 0)      //�⺻ ��� �̰�...
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