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
    //서버 연결
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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " : 로비 참가 성공!");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
    private void setActivation()
    {
        warnMsg.SetActive(!warnMsg.active);
    }
    //유효성 검사
    public void CheckName()
    {
        if (nickNameField.text.Length > 10 || nickNameField.text.Length < 2)       //10글자가 넘는 다면...
        {
            Debug.Log(nickNameField.text.Length + "글자수는 10자 미만 2글자 이상으로 해주세요.");
            setActivation();
        }
        else
        {       //조건 성립시 커넥트
            ConnectMsg.SetActive(true);
            Connect();
        }
    }

    public void func1()
    {
        if (warnMsg.active)
        {
            //활성화 상태에서 배경 터치시 제거
            if (Input.GetMouseButtonDown(0))
            {
                warnMsg.SetActive(false);
            }
        }
    }
}
