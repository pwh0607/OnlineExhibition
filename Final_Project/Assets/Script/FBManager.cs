using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;
using Photon.Pun;

public class FBManager : MonoBehaviourPun
{
    public static FBManager instance;

    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    private string currentRoomName;
        
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            currentRoomName = PhotonNetwork.CurrentRoom.Name;

            FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibitiontest-default-rtdb.firebaseio.com/");
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child("rooms").Child(currentRoomName);

            storage = FirebaseStorage.DefaultInstance;
            storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-6cf84.appspot.com").Child(currentRoomName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public DatabaseReference GetDatabaseReference()
    {
        return databaseRef;
    }

    public StorageReference GetStorageReference()
    {
        return storageRef;
    }
}
