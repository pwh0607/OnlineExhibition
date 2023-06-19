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
}