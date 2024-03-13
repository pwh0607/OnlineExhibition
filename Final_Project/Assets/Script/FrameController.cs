using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class FrameController : MonoBehaviourPun
{
    private string objName;
    private string roomName;

    public TMP_Text photoName;
    private void Awake()
    {
        RoomManager.instance.add_Frame();
        objName = "Frame" + RoomManager.instance.get_FrameCnt();
        roomName = PhotonNetwork.CurrentRoom.Name;
    }
    public string getObjName()
    {   
        Debug.Log("objName : " + objName);
        return objName;
    }
    public string getRoomName()
    {
        return roomName;
    }
    public void setPhotoName(string name)
    {
        Debug.Log(name + "test.....");
        photoName.SetText(name);
    }
}