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

            //스토리지 참조
            storage = FirebaseStorage.DefaultInstance;
            storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-6cf84.appspot.com").Child(currentRoomName);
            Debug.Log("FBManager 싱글톤....");
        }
        else
        {
            // 인스턴스가 이미 존재하면 현재 객체 파괴
            Destroy(gameObject);
        }
    }

    public DatabaseReference GetDatabaseReference()
    {
        Debug.Log("DB 참조 가져오기...");
        return databaseRef;
    }

    public StorageReference GetStorageReference()
    {
        Debug.Log("Storage 참조 가져오기...");
        return storageRef;
    }
}
