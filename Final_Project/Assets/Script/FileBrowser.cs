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

public class FileBrowser : MonoBehaviourPun
{
    private FirebaseStorage storage;
    private StorageReference storageRef;

    [SerializeField]
    RawImage rawImage;
    public GameObject cube;

    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-9dbef.appspot.com");
    }

    [PunRPC]
    private void OnMouseDown()
    {
        PhotonView pv = gameObject.GetComponent<PhotonView>();
        //pv.RPC("OnClickImageLoad", RpcTarget.All);
    }

    [PunRPC]
    public void OnClickImageLoad()
    {
        //���������� �̹��� ��������
        NativeGallery.GetImageFromGallery((file)=> {
            FileInfo img = new FileInfo(file);

            if (!string.IsNullOrEmpty(file))
            {
                //�ҷ�����
                StartCoroutine(UpLoadImage(file));
            }
        });

        //FB�� �̹��� ���ε� �ϱ�
        IEnumerator UpLoadImage(string imagePath)
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            string imageName = Path.GetFileName(imagePath).Split('.')[0];
            string saveImagePath = Application.persistentDataPath + "/Image";
            string imageFullName = imageName + ".jpg";

            File.WriteAllBytes(saveImagePath + imageFullName, imageData);

            //metadata����
            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            //�� �̸�


            StorageReference uploadRef = storageRef.Child("Room1/" + imageFullName);
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
            yield return null;
        }
    }
}
