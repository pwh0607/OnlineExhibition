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
    //FB 참조
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomName;          //FB storage 폴더 접근용.
    public GameObject cube;     //이미지

    //이미지 매핑
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
        //변수값 초기화
        roomName = "Room1";           //나중에 scene이름으로 변경.
        //데이터베이스 참조
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-9dbef-default-rtdb.firebaseio.com");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        //스토리지 참조
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-9dbef.appspot.com").Child(roomName);
    }

    //스토리지에서 이미지 가져오기
    private void GetImage(string photoPath) //파이어베이스 업로드
    {
        StorageReference image = storageRef.Child(photoPath);         //매개변수는 사진명
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(imageLoad(Convert.ToString(task.Result)));
            }
        });
    }

    //데이터베이스에서 사진path 가져오기
    public void ReadDB()
    {
        string objName = this.gameObject.GetComponent<FrameController>().getObjName();
        gameObject.GetComponent<FrameController>().setsibal(objName);
        RoomManager.instance.loadCheck();
        // 특정 데이터셋의 DB 참조 얻기
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
                    DataSnapshot data = task.Result;       //rooms의 자식객체인 room1을 참조.

                    // JSON 자체가 딕셔너리 기반   key는 오브젝트 이름(frame(n)) 값이 사진path;
                    IDictionary dirInfo = (IDictionary)data.Value;
                    string photoPath = Convert.ToString(dirInfo["photoPath"]);
                    GetImage(photoPath);
                 }
             }
         );
    }
}
