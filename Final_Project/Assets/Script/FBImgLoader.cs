using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;
using Photon.Pun;

public class FBImgLoader : MonoBehaviourPun
{
    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    public GameObject cube;         
    string roomName;               
    string objName;                 

    FBManager fbManager;
    public void FBinit()
    {
        fbManager = FBManager.instance;

        if (fbManager != null)
        {
            databaseRef = fbManager.GetDatabaseReference();
            storageRef = fbManager.GetStorageReference();
        }
    }

    public void ReadDB()
    {
        objName = GetComponent<FrameController>().getObjName();

        databaseRef.Child(GetComponent<FrameController>().getObjName()).GetValueAsync().ContinueWith(
             task =>
             {
                 if (task.IsFaulted)
                 {
                     return;
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot data = task.Result;
                     if (data.Value == null)
                     {
                         return;
                     }
                     IDictionary dirInfo = (IDictionary)data.Value;
                     string photoPath = Convert.ToString(dirInfo["photoPath"]);
                     GetImage(photoPath);
                 }
             }
         );
    }

    public void GetImage(string photoPath)
    {
        StorageReference image = storageRef.Child(photoPath);
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(DownloadImage(Convert.ToString(task.Result)));
                string photoName = photoPath.Substring(0, photoPath.Length - 4);
            }
        });
    }

    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            cube.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        }
    }
        
    private void Awake()
    {
        FBinit();
    }
    void Start()
    {
        ReadDB();
    }
}