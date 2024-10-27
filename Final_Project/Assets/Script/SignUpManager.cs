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

        passwordField.contentType = InputField.ContentType.Password;
    }

    void Update()
    {
        closeWarnMsg();
    }

    public async void signUp()
    {
        string email = emailField.text;
        string password = passwordField.text;
        string nickname = nicknameField.text;

        var userTask = await authRef.CreateUserWithEmailAndPasswordAsync(email, password);
        FirebaseUser newUser = userTask.User;

        string uid = newUser.UserId;

        docRef = store.Collection("users").Document(uid);
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "uid", uid },
            { "email", email },
            { "nickname", nickname }
        };
        await docRef.SetAsync(userData);
    }

    public void closeTab()
    {
        signUpPart.SetActive(false);
    }

    public void closeWarnMsg()
    {
        if (warnMsg.activeSelf && Input.GetMouseButtonDown(0))
        {
            warnMsg.SetActive(false);
        }
    }

    public void CheckName()
    {
        if (nicknameField.text.Length > 10 || nicknameField.text.Length < 2)       
        {
            warnMsg.SetActive(true);
        }
        else
        {
            signUp();
            signUpPart.SetActive(false);
        }
    }
}
