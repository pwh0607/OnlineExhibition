using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;
public class FBAuthManager : MonoBehaviour
{
    public static FBAuthManager instance;

    public FirebaseAuth authRef;
    public FirebaseFirestore store;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            authRef = FirebaseAuth.DefaultInstance;
            store = FirebaseFirestore.DefaultInstance;
        }
        else
        {
            // 인스턴스가 이미 존재하면 현재 객체 파괴
            Destroy(gameObject);
        }
    }

    public FirebaseAuth getAuthRef()
    {
        if(authRef == null)
        {
            Debug.Log("인증 오류");
        }
        return authRef;
    }

    public FirebaseFirestore getStoreRef()
    {
        return store;
    }
}
