using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private string userNickname;
    public InputField nickNameField;
    public GameObject warnMsg;
    public Button startBtn;

    private void Start()
    {
        warnMsg.SetActive(false);
    }
    //���� ����
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        startBtn.interactable = false;
        PhotonNetwork.LocalPlayer.NickName = nickNameField.text;
    }
   public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+" : �κ� ���� ����!");
        PhotonNetwork.LoadLevel("Lobby");
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
            Invoke("setActivation", 2.0f);
        }
        else
        {       //���� ������ Ŀ��Ʈ
            Connect();
        }
    }
}