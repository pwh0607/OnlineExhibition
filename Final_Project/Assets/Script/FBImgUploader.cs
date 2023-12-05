using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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

    string roomName;          //FB storage 폴더 접근용.
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

    //데이터 베이스 업로드
    public void DBUpload(string imageFullName)
    {
        string objName = this.gameObject.GetComponent<FrameController>().getObjName();
        string photoPath = imageFullName.Trim();

        var data = new Photo(photoPath);
        string json = JsonUtility.ToJson(data);

        databaseRef.Child(objName).SetRawJsonValueAsync(json);
    }

    //스토리지에 이미지 업로드
    public void OnClickImageLoad()
    {
        NativeGallery.GetImageFromGallery((file) => {
            FileInfo img = new FileInfo(file);

            if (!string.IsNullOrEmpty(file))
            {
                //불러오기
                StartCoroutine(UpLoadImage(file));
            }
        });

        IEnumerator UpLoadImage(string imagePath)
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            string imageName = Path.GetFileName(imagePath).Split('.')[0];
            string saveImagePath = Application.persistentDataPath + "/Image";
            string imageFullName = imageName + ".jpg";

            File.WriteAllBytes(saveImagePath + imageFullName, imageData);

            DBUpload(imageFullName);                //이미지path DB에 업로드
            //metadata세팅
            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child(roomName + "/" + imageFullName);        //수정 필요!
            uploadRef.PutBytesAsync(imageData).ContinueWithOnMainThread((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("실패...");
                }
                else
                {
                    Debug.Log("성공!!");

                    //동기화 메서드 이므로 위의 행위가 끝난 후로 실행해야함.
                    GetComponent<FBImgLoader>().ReadDB();
                }
            });
            yield return null;
        }
    }

    void FBinit()
    {
        roomName = GetComponent<FrameController>().getRoomName().Trim();         //양 옆 공백 제거

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://onlineexhibition-fd6aa-default-rtdb.firebaseio.com/");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        databaseRef = databaseRef.Child("rooms").Child(roomName);
        
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-fd6aa.appspot.com");   
    }
    void Start()
    {
        FBinit();
    }
}