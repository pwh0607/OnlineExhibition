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

        if(fbManager != null)
        {
            databaseRef = fbManager.GetDatabaseReference();
            storageRef = fbManager.GetStorageReference();
        }
    }

    public void DBUpload()
    {
        string objName = this.gameObject.GetComponent<FrameController>().getObjName();
        string photoPath = imageFullName.Trim();

        var data = new Photo(photoPath);
        string json = JsonUtility.ToJson(data);

        databaseRef.Child(objName).SetRawJsonValueAsync(json);
    }

    public void OnClickImageLoad()
    {
        NativeGallery.GetImageFromGallery(file => {
            FileInfo img = new FileInfo(file);
            if (!string.IsNullOrEmpty(file))
            {
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
            DBUpload();

            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child(imageFullName);
            uploadRef.PutBytesAsync(imageData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    return;
                }
                GetComponent<FBImgLoader>().ReadDB();
            });
            yield return null;
        }
    }

    void DeleteDB()
    {
        string objName = GetComponent<FrameController>().getObjName();
        DatabaseReference del_ref = databaseRef.Child(objName);

        del_ref.RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
        });
    }

    void DeleteStorage()
    {
        string objName = GetComponent<FrameController>().getObjName();
        StorageReference del_ref = storageRef.Child(imageFullName);

        del_ref.DeleteAsync().ContinueWithOnMainThread(task => { });
    }
    
    void Start()
    {
        FBinit();
    }
}