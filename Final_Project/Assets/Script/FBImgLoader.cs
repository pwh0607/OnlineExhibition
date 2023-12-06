using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

public class FBImgLoader : MonoBehaviour
{
    //FB ����
    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    string roomName;          //FB storage ���� ���ٿ�.
    public GameObject cube;     //�̹���
    string objName;
    public void FBinit()
    {
        //������ �ʱ�ȭ
        roomName = GetComponent<FrameController>().getRoomName().Trim();           //���߿� scene�̸����� ����.
        objName = this.gameObject.GetComponent<FrameController>().getObjName();

        //�����ͺ��̽� ����
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibitiontest-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child("rooms").Child(roomName);

        //���丮�� ����
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-fd6aa.appspot.com").Child(roomName);
    }
    //���丮������ �̹��� ��������
    public void GetImage(string photoPath)
    {
        StorageReference image = storageRef.Child(photoPath);

        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("�̹��� �������� ����.");
                StartCoroutine(DownloadImage(Convert.ToString(task.Result)));
            }
            else
            {
                Debug.LogError("�̹��� �������� ����: " + task.Exception);
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
        else
        {
            Debug.LogError("�̹��� �ٿ�ε� ����: " + request.error);
        }
    }

    //�����ͺ��̽����� photoPath ��������
    public void ReadDB()
    {
        databaseRef.Child(objName).GetValueAsync().ContinueWith(
             task =>
             {
                 if (task.IsFaulted)
                 {
                     Debug.Log("Load Data Faulted");
                     return;
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot data = task.Result;
                     //�ش� data value�� ������.... �Ѿ��. (�ش� ���� DB�� ���ٴ� ��.)
                     if(data.Value ==  null)
                     {
                         Debug.Log("null �Դϴ�.");
                         return;
                     }

                     //������ �������� ������...
                     Debug.Log("Load Data Success!");

                     // JSON ��ü�� ��ųʸ� ���   key�� ������Ʈ �̸�(frame(n)) ���� ����path;
                     IDictionary dirInfo = (IDictionary)data.Value;
                     string photoPath = Convert.ToString(dirInfo["photoPath"]); 
                     Debug.Log("�̹��� �ҷ�����...");
                     GetImage(photoPath);
                 }
             }
         );
    }
    private void Awake()
    {
        //���丮�� ����
        FBinit();
    }
    void Start()
    {
        ReadDB();
    }
}