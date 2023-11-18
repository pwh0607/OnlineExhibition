using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

public class FBImgUploader : MonoBehaviour
{
    //FB ����
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomNumber;          //FB storage ���� ���ٿ�.
    //public GameObject cube;     //�̹���

    public class Photo
    {
        public string photoPath;
        public Photo()
        {

        }
        public Photo(string photoPath)
        {
            this.photoPath = photoPath;
        }
    }

    private void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-9dbef-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void DBUpload()
    {
        string objName = gameObject.name.Trim();
        string photoPath = "obj1.jpg".Trim();

        var data = new Photo(photoPath);
        string json = JsonUtility.ToJson(data);

        databaseRef.Child("rooms").Child("room1").Child(objName).SetRawJsonValueAsync(json);

        Debug.Log(json);
        Debug.Log("DB ���� �Ϸ�!");
    }
}