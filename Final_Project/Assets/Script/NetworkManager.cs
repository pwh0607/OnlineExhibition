using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string userNickname;
    public InputField nickNameField;
    public GameObject warnMsg;
    public GameObject ConnectMsg;
    public Button startBtn;

    private void Start()
    {
        warnMsg.SetActive(false);
        ConnectMsg.SetActive(false);
    }
    //���� ����
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = nickNameField.text;
        startBtn.interactable = false;
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " : �κ� ���� ����!");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
    private void setActivation()
    {
        warnMsg.SetActive(!warnMsg.active);
    }
    //��ȿ�� �˻�
    public void CheckName()
    {
        if (nickNameField.text.Length > 10 || nickNameField.text.Length < 2)       //10���ڰ� �Ѵ� �ٸ�...
        {
            Debug.Log(nickNameField.text.Length + "���ڼ��� 10�� �̸� 2���� �̻����� ���ּ���.");
            setActivation();
        }
        else
        {       //���� ������ Ŀ��Ʈ
            ConnectMsg.SetActive(true);
            Connect();
        }
    }

    public void func1()
    {
        if (warnMsg.active)
        {
            //Ȱ��ȭ ���¿��� ��� ��ġ�� ����
            if (Input.GetMouseButtonDown(0))
            {
                warnMsg.SetActive(false);
            }
        }
    }
}
