using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;
using Photon.Pun;

public class FBImgUploader : MonoBehaviourPun
{
    //FB 참조
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string imageFullName;

    FBManager fbManager;
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

    void FBinit()
    {
        fbManager = FBManager.instance;

        //test
        if(fbManager != null)
        {
            databaseRef = fbManager.GetDatabaseReference();
            storageRef = fbManager.GetStorageReference();
        }
        else
        {
            Debug.Log("FB manager null...'");
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
            imageFullName = imageName + ".jpg";

            File.WriteAllBytes(saveImagePath + imageFullName, imageData);
            DBUpload(imageFullName);

            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child(imageFullName);
            uploadRef.PutBytesAsync(imageData).ContinueWithOnMainThread((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("업로드 실패...");
                }
                else
                {
                    Debug.Log("업로드 성공!!");
                    //동기화 메서드 이므로 위의 행위가 끝난 후로 실행해야함.
                    GetComponent<FBImgLoader>().ReadDB();
                }
            });
            yield return null;
        }
    }

    //DB에서 데이터 삭제.(액자 삭제시...)
    void DeleteDB()
    {
        string objName = GetComponent<FrameController>().getObjName();
        DatabaseReference del_ref = databaseRef.Child(objName);

        del_ref.RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("DB : DB data Delete Error");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("DB : Object data Delete 완료...");
            }
        });
    }

    void DeleteStorage()
    {
        string objName = GetComponent<FrameController>().getObjName();
        StorageReference del_ref = storageRef.Child(imageFullName);

        del_ref.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("STORAGE : image 삭제 완료");
            }
            else
            {
                Debug.Log("STORAGE : image 삭제 실패");
            }
        });
    }

    /*
    private void OnDestroy()
    {
        //오브젝트 삭제 콜백.
        DeleteDB();
        DeleteStorage();
    }
    */
    
    void Start()
    {
        FBinit();
    }
}