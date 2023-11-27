using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FrameController : MonoBehaviour
{
    private string objName;
    public GameObject tmp;
    private void Awake()
    {
        RoomManager.instance.add_Frame();
        objName = "Frame" + RoomManager.instance.get_FrameCnt();
        gameObject.GetComponent<FBImgLoader>().ReadDB();

    }
    public void setsibal(string name)
    {
        tmp.GetComponent<TextMeshPro>().text = name;
    }
    public string getObjName()
    {
        return objName;
    }
}