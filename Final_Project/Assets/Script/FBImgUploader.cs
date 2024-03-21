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
    //FB ����
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

    //������ ���̽� ���ε�
    public void DBUpload(string imageFullName)
    {
        string objName = this.gameObject.GetComponent<FrameController>().getObjName();
        string photoPath = imageFullName.Trim();

        var data = new Photo(photoPath);
        string json = JsonUtility.ToJson(data);

        databaseRef.Child(objName).SetRawJsonValueAsync(json);
    }

    //���丮���� �̹��� ���ε�
    public void OnClickImageLoad()
    {
        NativeGallery.GetImageFromGallery((file) => {
            FileInfo img = new FileInfo(file);
            if (!string.IsNullOrEmpty(file))
            {
                //�ҷ�����
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
                    Debug.Log("���ε� ����...");
                }
                else
                {
                    Debug.Log("���ε� ����!!");
                    //����ȭ �޼��� �̹Ƿ� ���� ������ ���� �ķ� �����ؾ���.
                    GetComponent<FBImgLoader>().ReadDB();
                }
            });
            yield return null;
        }
    }

    //DB���� ������ ����.(���� ������...)
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
                Debug.Log("DB : Object data Delete �Ϸ�...");
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
                Debug.Log("STORAGE : image ���� �Ϸ�");
            }
            else
            {
                Debug.Log("STORAGE : image ���� ����");
            }
        });
    }

    /*
    private void OnDestroy()
    {
        //������Ʈ ���� �ݹ�.
        DeleteDB();
        DeleteStorage();
    }
    */
    
    void Start()
    {
        FBinit();
    }
}