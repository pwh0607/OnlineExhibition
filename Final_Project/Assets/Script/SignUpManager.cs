using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;

public class SignUpManager : MonoBehaviour
{
    public FirebaseAuth authRef;
    public FirebaseFirestore store;
    public DocumentReference docRef;

    public InputField emailField;
    public InputField passwordField;
    public InputField nicknameField;

    public GameObject signUpPart;
    public GameObject warnMsg;
    
    void Start()
    {
        authRef = FBAuthManager.instance.getAuthRef();
        store = FBAuthManager.instance.getStoreRef();

        //password 세팅
        passwordField.contentType = InputField.ContentType.Password;
    }

    void Update()
    {
        closeWarnMsg();
    }

    public async void signUp()        //완료 버튼 클릭시...
    {
        string email = emailField.text;
        string password = passwordField.text;
        string nickname = nicknameField.text;

        try
        {
            // Firebase 사용자 생성
            var userTask = await authRef.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = userTask.User;

            string uid = newUser.UserId;

            // Firestore에 사용자 정보 저장
            docRef = store.Collection("users").Document(uid);
            Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "uid", uid },
                { "email", email },
                { "nickname", nickname }
            };
            await docRef.SetAsync(userData);

            Debug.Log("회원가입 성공! 이메일: " + email + ", 닉네임: " + nickname);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("회원가입 실패: " + ex.Message);
        }
    }

    //UI
    public void closeTab()
    {
        signUpPart.SetActive(false);
    }

    public void closeWarnMsg()
    {
        //활성화 상태에서 배경 터치시 제거
        if (warnMsg.activeSelf && Input.GetMouseButtonDown(0))
        {
            warnMsg.SetActive(false);
        }
    }
    //유효성 검사
    public void CheckName()
    {
        if (nicknameField.text.Length > 10 || nicknameField.text.Length < 2)       //10글자가 넘는 다면...
        {
            //Debug.Log(emailField.text.Length + "글자수는 10자 미만 2글자 이상으로 해주세요.");
            warnMsg.SetActive(true);
        }
        else
        {
            //조건 성립시 커넥트

            signUp();
            signUpPart.SetActive(false);
        }
    }
}
