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
    //FB ����

    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    string roomNumber;          //FB storage ���� ���ٿ�.
    //public GameObject cube;     //�̹���

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

    //������ ���̽� ���ε�
    public void DBUpload(string roomName, string imageFullName)
    {
        string objName = gameObject.name.Trim();
        string photoPath = imageFullName.Trim();

        var data = new Photo(photoPath);
        string json = JsonUtility.ToJson(data);

        databaseRef.Child("rooms").Child(roomName).Child(objName).SetRawJsonValueAsync(json);

        Debug.Log(json);
        Debug.Log("DB ���� �Ϸ�!");

        //������ �� ������Ʈ�� �̹��� update 
        gameObject.GetComponent<FBImgLoader>().ReadDB();
    }

    //�̹��� ���ε�
    public void OnClickImageLoad()
    {
        NativeGallery.GetImageFromGallery((file) => {
            FileInfo img = new FileInfo(file);

            if (!string.IsNullOrEmpty(file))
            {
                //�ҷ�����
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
            
            //metadata����
            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child("Room1/" + imageFullName);        //���� �ʿ�!
            uploadRef.PutBytesAsync(imageData).ContinueWithOnMainThread((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("����...");
                }
                else
                {
                    Debug.Log("����!!");
                }
            });

            DBUpload("Room1", imageFullName);
            yield return null;
        }
    }
}