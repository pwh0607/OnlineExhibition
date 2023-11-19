using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using Photon.Pun;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

public class FBImgUploader : MonoBehaviour
{
    //FB 참조

    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomNumber;          //FB storage 폴더 접근용.
    //public GameObject cube;     //이미지

    public class Photo
    {
        public string photoPath;
        public Photo()
        {

        }
        public Photo(string photoPath)
        {
            this.photoPath = photoPath;
        }
    }

    private void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-9dbef-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-9dbef.appspot.com");
    }

    //데이터 베이스 업로드
    public void DBUpload(string roomName, string imageFullName)
    {
        string objName = gameObject.name.Trim();
        string photoPath = imageFullName.Trim();

        var data = new Photo(photoPath);
        string json = JsonUtility.ToJson(data);

        databaseRef.Child("rooms").Child(roomName).Child(objName).SetRawJsonValueAsync(json);

        Debug.Log(json);
        Debug.Log("DB 저장 완료!");

        //저장후 씬 오브젝트에 이미지 update 
        gameObject.GetComponent<FBImgLoader>().ReadDB();
    }

    //이미지 업로드
    public void OnClickImageLoad()
    {
        NativeGallery.GetImageFromGallery((file) => {
            FileInfo img = new FileInfo(file);

            if (!string.IsNullOrEmpty(file))
            {
                //불러오기
                StartCoroutine(LoadImage(file));
            }
        });

        IEnumerator LoadImage(string imagePath)
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            string imageName = Path.GetFileName(imagePath).Split('.')[0];
            string saveImagePath = Application.persistentDataPath + "/Image";
            string imageFullName = imageName + ".jpg";

            File.WriteAllBytes(saveImagePath + imageFullName, imageData);
            
            //metadata세팅
            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child("Room1/" + imageFullName);        //수정 필요!
            uploadRef.PutBytesAsync(imageData).ContinueWithOnMainThread((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("실패...");
                }
                else
                {
                    Debug.Log("성공!!");
                }
            });

            DBUpload("Room1", imageFullName);
            yield return null;
        }
    }
}