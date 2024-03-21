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

        //password ����
        passwordField.contentType = InputField.ContentType.Password;
    }

    void Update()
    {
        closeWarnMsg();
    }

    public async void signUp()        //�Ϸ� ��ư Ŭ����...
    {
        string email = emailField.text;
        string password = passwordField.text;
        string nickname = nicknameField.text;

        try
        {
            // Firebase ����� ����
            var userTask = await authRef.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = userTask.User;

            string uid = newUser.UserId;

            // Firestore�� ����� ���� ����
            docRef = store.Collection("users").Document(uid);
            Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "uid", uid },
                { "email", email },
                { "nickname", nickname }
            };
            await docRef.SetAsync(userData);

            Debug.Log("ȸ������ ����! �̸���: " + email + ", �г���: " + nickname);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ȸ������ ����: " + ex.Message);
        }
    }

    //UI
    public void closeTab()
    {
        signUpPart.SetActive(false);
    }

    public void closeWarnMsg()
    {
        //Ȱ��ȭ ���¿��� ��� ��ġ�� ����
        if (warnMsg.activeSelf && Input.GetMouseButtonDown(0))
        {
            warnMsg.SetActive(false);
        }
    }
    //��ȿ�� �˻�
    public void CheckName()
    {
        if (nicknameField.text.Length > 10 || nicknameField.text.Length < 2)       //10���ڰ� �Ѵ� �ٸ�...
        {
            //Debug.Log(emailField.text.Length + "���ڼ��� 10�� �̸� 2���� �̻����� ���ּ���.");
            warnMsg.SetActive(true);
        }
        else
        {
            //���� ������ Ŀ��Ʈ

            signUp();
            signUpPart.SetActive(false);
        }
    }
}
