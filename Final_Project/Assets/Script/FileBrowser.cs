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
    string filePath;
    private Texture2D texture;

    private FirebaseStorage storage;
    private StorageReference storageRef;

    [SerializeField]
    RawImage rawImage;
    public GameObject cube;

    private void Start()
    {
        //  rawImage = cube.GetComponent<RawImage>();
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://onlineexhibition-9dbef.appspot.com");
    }

    [PunRPC]
    private void OnMouseDown()
    {
        PhotonView pv = gameObject.GetComponent<PhotonView>();
        pv.RPC("OnClickImageLoad", RpcTarget.All);
    }

    [PunRPC]
    public void OnClickImageLoad()
    {
        NativeGallery.GetImageFromGallery((file)=> {
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

            File.WriteAllBytes(saveImagePath + imageName + ".jpg", imageData);

            var tempImage = File.ReadAllBytes(imagePath);

            //이미지 매핑용파트
            /*
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(tempImage);
            cube.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
            */
            //metadata세팅
            var metaData = new MetadataChange();
            metaData.ContentType = "image/jpg";

            StorageReference uploadRef = storageRef.Child("Room1/newFile.jpg");
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

            yield return null;
        }
    }
}
