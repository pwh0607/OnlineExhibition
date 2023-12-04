using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

public class FBTesting : MonoBehaviour
{
    //FB ����
    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    string roomName;          //FB storage ���� ���ٿ�.
    public GameObject cube;     //�̹���
    string objName;

    //�̹��� ����
    IEnumerator imageLoad(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = new Texture2D(0, 0);
            texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            cube.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        }
    }
    public void FBinit()
    {
        Debug.Log("Ȯ�� 1");
        //������ �ʱ�ȭ

        //�����ͺ��̽� ����
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-fd6aa-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child("rooms").Child("Room1").Child("Frame1");

    }

    //���丮������ �̹��� ��������
    private void GetImage(string photoPath) //���̾�̽� ���ε�
    {
        Debug.Log("Ȯ�� 2");
        StorageReference image = storageRef.Child("photo.jpg");         //�Ű������� ������
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Ȯ�� 3");
                StartCoroutine(imageLoad(Convert.ToString(task.Result)));
            }
        });
    }

    //�����ͺ��̽����� ����path ��������
    public void ReadDB()
    {
        databaseRef.GetValueAsync().ContinueWith(
             task =>
             {
                 if (task.IsFaulted)
                 {
                     Debug.Log("Load Data Faulted");
                 }
                 else if (task.IsCompleted)
                 {
                     Debug.Log("Load Data Success!");
                     DataSnapshot data = task.Result;       //rooms�� �ڽİ�ü�� room1�� ����.

                     // JSON ��ü�� ��ųʸ� ���   key�� ������Ʈ �̸�(frame(n)) ���� ����path;
                     IDictionary dirInfo = (IDictionary)data.Value;
                     string photoPath = Convert.ToString(dirInfo["photoPath"]);
                     Debug.Log("PP :" + photoPath);
                     //GetImage(photoPath);
                 }
             }
         );
    }

    void Start()
    {
        roomName = "Room1";           //���߿� scene�̸����� ����.
     
        //���丮�� ����
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-fd6aa.appspot.com").Child(roomName);
        GetImage("photo.jpg");
    }
}
