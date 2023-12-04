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

    //�̹��� ����
    IEnumerator imageLoad(string MediaUrl)
    {
        //RoomManager.instance.loadCheck();
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
    public void GetImage(string photoPath) //���̾�̽� ���ε�
    {   
        StorageReference image = storageRef.Child(photoPath);         //�Ű������� ������
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("�̹��� �������� ����.");
                StartCoroutine(imageLoad(Convert.ToString(task.Result)));
            }
        });
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
    void Start()
    {
        //���丮�� ����
        FBinit();
        ReadDB();
    }
}