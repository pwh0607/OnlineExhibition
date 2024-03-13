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
    //FB ����
    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    string roomName;          //FB storage ���� ���ٿ�.
    public GameObject cube;     //�̹���
    //���� ������ ��ȣ.
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
        else
        {
            Debug.Log("FB manager null...'");
        }
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

                string photoName = photoPath.Substring(0, photoPath.Length - 4);
                Debug.Log(photoName);
                gameObject.GetComponent<FrameController>().setPhotoName(photoName);

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
        objName = GetComponent<FrameController>().getObjName();
        databaseRef.Child(GetComponent<FrameController>().getObjName()).GetValueAsync().ContinueWith(
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