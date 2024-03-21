using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

using System;
using System.Threading.Tasks;

using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public FirebaseAuth authRef;
    public FirebaseFirestore store;
    public DocumentReference docRef;

    private string userNickname;
    public InputField emailField;
    public InputField passwordField;

    public GameObject connectMsg;
    public GameObject signUpPart;
    public Button startBtn;

    private void Awake()
    {
        authRef = FBAuthManager.instance.getAuthRef();
        store = FBAuthManager.instance.getStoreRef();
    }
    private void Start()
    {
        connectMsg.SetActive(false);
        //패스워드 형식으로 세팅
        passwordField.contentType = InputField.ContentType.Password;
    }
    //서버 연결
    public void Connect()
    {
        startBtn.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
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

    public void setSignUpWindow()
    {
        signUpPart.SetActive(true);
    }

    public async void login()
    {
        string email = emailField.text;
        string password = passwordField.text;
        connectMsg.SetActive(true);
        try
        {
            await authRef.SignInWithEmailAndPasswordAsync(email, password);
            if (authRef.CurrentUser != null)
            {
                string uid = authRef.CurrentUser.UserId;
                await getNickname(uid);             //해당 비동기 작업 수행후 아래의 코드 실행
                Connect();
            }
         }catch(Exception e){
            Debug.Log("로그인 실패 : " + e.Message);
            connectMsg.SetActive(false);
        }
    }

    async Task getNickname(string uid)
    {
        try
        {     
            docRef = store.Collection("users").Document(uid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                string nickname = snapshot.GetValue<string>("nickname");
                PhotonNetwork.NickName = nickname;
            }
        }catch(Exception e)
        {
            Debug.Log("닉네임 읽기 실패 : " + e.Message);
        }

    }
}
