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
using Photon.Pun;
using Photon.Realtime;

public class FBImgLoader : MonoBehaviourPunCallbacks
{
    //FB ����
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomName;          //FB storage ���� ���ٿ�.
    public GameObject cube;     //�̹���

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

    private void Awake()
    {
        //������ �ʱ�ȭ
        roomName = "Room1";           //���߿� scene�̸����� ����.
        //�����ͺ��̽� ����
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-9dbef-default-rtdb.firebaseio.com");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        //���丮�� ����
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-9dbef.appspot.com").Child(roomName);
    }

    //���丮������ �̹��� ��������
    private void GetImage(string photoPath) //���̾�̽� ���ε�
    {
        StorageReference image = storageRef.Child(photoPath);         //�Ű������� ������
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(imageLoad(Convert.ToString(task.Result)));
            }
        });
    }

    //�����ͺ��̽����� ����path ��������
    public void ReadDB()
    {
        string objName = this.gameObject.GetComponent<FrameController>().getObjName();
        gameObject.GetComponent<FrameController>().setsibal(objName);
        RoomManager.instance.loadCheck();
        // Ư�� �����ͼ��� DB ���� ���
        databaseRef.Child("rooms").Child(roomName).Child(objName).GetValueAsync().ContinueWith(
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
                    GetImage(photoPath);
                 }
             }
         );
    }
}
