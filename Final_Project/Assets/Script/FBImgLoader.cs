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

public class FBImgLoader : MonoBehaviour
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

    void Start()
    {
        //������ �ʱ�ȭ
        roomName = "Room1";           //���߿� scene�̸����� ����.

        //�����ͺ��̽� ����
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-9dbef-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child("rooms").Child(roomName).Child(gameObject.name);

        //���丮�� ����
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-9dbef.appspot.com").Child(roomName);

        //���۰� ���ÿ� DB�� �о ������Ʈ�� ���� ����.
        ReadDB();
    }
    
    //���丮������ �̹��� ��������
    public void GetImage(string photoPath) //���̾�̽� ���ε�
    {
        StorageReference image = storageRef.Child(photoPath);         //�Ű������� ������
        Debug.Log(image.Path + " ������� ����!");

        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("����!!");
                StartCoroutine(imageLoad(Convert.ToString(task.Result)));
            }
        });
    }

    //�����ͺ��̽����� ����path ��������
    public void ReadDB()
    {
        // Ư�� �����ͼ��� DB ���� ���


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
                    Debug.Log("photo 1 : " + dirInfo["photoPath"]);
                    string photoPath = Convert.ToString(dirInfo["photoPath"]);
                    GetImage(photoPath);
                 }
             }
         );
    }
}
