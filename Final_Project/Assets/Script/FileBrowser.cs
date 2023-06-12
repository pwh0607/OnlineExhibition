using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FileBrowser : MonoBehaviour
{
    string filePath;
    private Texture2D texture;
    public GameObject cube;
    public GameObject text;
    public void OpenBrowser()
    {
        filePath = EditorUtility.OpenFilePanel("Overwrite with png", "Application.streamingAssetsPath", "png");

        if (filePath.Length != 0)
        {
            WWW www = new WWW("file://" + filePath);
            texture = new Texture2D(64, 64);
            www.LoadImageIntoTexture(texture);
            cube.GetComponent<Renderer>().material.mainTexture = texture;
            text.SetActive(false);
        }
    }
}
