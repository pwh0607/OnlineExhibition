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
    //FB 참조
    FirebaseStorage storage;
    StorageReference storageRef;
    DatabaseReference databaseRef;

    string roomName;          //FB storage 폴더 접근용.
    public GameObject cube;     //이미지
    string objName;

    //이미지 매핑
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
        //변수값 초기화
        roomName = GetComponent<FrameController>().getRoomName().Trim();           //나중에 scene이름으로 변경.
        objName = this.gameObject.GetComponent<FrameController>().getObjName();

        //데이터베이스 참조
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibitiontest-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child("rooms").Child(roomName);

        //스토리지 참조
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-fd6aa.appspot.com").Child(roomName);
    }
    //스토리지에서 이미지 가져오기
    public void GetImage(string photoPath) //파이어베이스 업로드
    {   
        StorageReference image = storageRef.Child(photoPath);         //매개변수는 사진명
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("이미지 가져오기 성공.");
                StartCoroutine(imageLoad(Convert.ToString(task.Result)));
            }
        });
    }
    //데이터베이스에서 photoPath 가져오기
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
                     Debug.Log("이미지 불러오기...");
                     GetImage(photoPath);
                 }
             }
         );
    }
    void Start()
    {
        //스토리지 참조
        FBinit();
        ReadDB();
    }
}