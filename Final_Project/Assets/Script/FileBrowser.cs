using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using Photon.Pun;

public class FileBrowser : MonoBehaviourPun
{
    string filePath;
    private Texture2D texture;

    [SerializeField]
    RawImage rawImage;
    public GameObject cube;

    private void Start()
    {
        rawImage = cube.GetComponent<RawImage>();
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
                //�ҷ�����
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

            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(tempImage);
            cube.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
            yield return null;
        }
    }
}
