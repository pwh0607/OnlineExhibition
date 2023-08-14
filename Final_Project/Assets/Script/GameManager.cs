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

    //서버 연결
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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+" : 로비 참가 성공!");
        PhotonNetwork.LoadLevel("Lobby");
    }

    //유효성 검사
    public void CheckName()
    {
        if (nickNameField.text.Length > 10 || nickNameField.text.Length < 2)       //10글자가 넘는 다면...
        {
            Debug.Log(nickNameField.text.Length + "글자수는 10자 미만 2글자 이상으로 해주세요.");
        }
        else
        {       //조건 성립시 커넥트
            Connect();
        }
    }
}