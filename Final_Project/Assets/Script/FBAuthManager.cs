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
            Destroy(gameObject);
        }
    }

    public FirebaseAuth getAuthRef()
    {
        return authRef;
    }

    public FirebaseFirestore getStoreRef()
    {
        return store;
    }
}
