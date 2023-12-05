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
    //FB ����
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomName;          //FB storage ���� ���ٿ�.
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
            string imageFullName = imageName + ".jpg";

            File.WriteAllBytes(saveImagePath + imageFullName, imageData);

            DBUpload(imageFullName);                //�̹���path DB�� ���ε�
            //metadata����
            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child(roomName + "/" + imageFullName);        //���� �ʿ�!
            uploadRef.PutBytesAsync(imageData).ContinueWithOnMainThread((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("����...");
                }
                else
                {
                    Debug.Log("����!!");

                    //����ȭ �޼��� �̹Ƿ� ���� ������ ���� �ķ� �����ؾ���.
                    GetComponent<FBImgLoader>().ReadDB();
                }
            });
            yield return null;
        }
    }

    void FBinit()
    {
        roomName = GetComponent<FrameController>().getRoomName().Trim();         //�� �� ���� ����

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