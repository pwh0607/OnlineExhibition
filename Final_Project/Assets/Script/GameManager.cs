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
    public Button startBtn;

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

    //��ȿ�� �˻�
    public void CheckName()
    {
        if (nickNameField.text.Length > 10 || nickNameField.text.Length < 2)       //10���ڰ� �Ѵ� �ٸ�...
        {
            Debug.Log(nickNameField.text.Length + "���ڼ��� 10�� �̸� 2���� �̻����� ���ּ���.");
        }
        else
        {       //���� ������ Ŀ��Ʈ
            Connect();
        }
    }
}