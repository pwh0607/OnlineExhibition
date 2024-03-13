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
    //FB 참조
    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    string roomName;          //FB storage 폴더 접근용.
    public GameObject cube;     //이미지
    //현재 액자의 번호.
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
    //스토리지에서 이미지 가져오기
    public void GetImage(string photoPath)
    {
        StorageReference image = storageRef.Child(photoPath);
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("이미지 가져오기 성공.");
                StartCoroutine(DownloadImage(Convert.ToString(task.Result)));

                string photoName = photoPath.Substring(0, photoPath.Length - 4);
                Debug.Log(photoName);
                gameObject.GetComponent<FrameController>().setPhotoName(photoName);

            }
            else
            {
                Debug.LogError("이미지 가져오기 실패: " + task.Exception);
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
            Debug.LogError("이미지 다운로드 실패: " + request.error);
        }
    }

    //데이터베이스에서 photoPath 가져오기
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
                     //해당 data value가 없으면.... 넘어간다. (해당 값이 DB에 없다는 뜻.)
                     if(data.Value ==  null)
                     {
                         Debug.Log("null 입니다.");
                         return;
                     }

                     //데이터 가져오기 성공시...
                     Debug.Log("Load Data Success!");
                     // JSON 자체가 딕셔너리 기반   key는 오브젝트 이름(frame(n)) 값이 사진path;
                     IDictionary dirInfo = (IDictionary)data.Value;
                     string photoPath = Convert.ToString(dirInfo["photoPath"]);
                     GetImage(photoPath);
                 }
             }
         );
    }
    private void Awake()
    {
        //스토리지 참조
        FBinit();
    }
    void Start()
    {
        ReadDB();
    }
}