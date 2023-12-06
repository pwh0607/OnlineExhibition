using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FrameController : MonoBehaviourPun
{
    private string objName;
    private string roomName;
    private void Awake()
    {
        RoomManager.instance.add_Frame();
        objName = "Frame" + RoomManager.instance.get_FrameCnt();
        roomName = PhotonNetwork.CurrentRoom.Name;
    }
    public string getObjName()
    {
        return objName;
    }
    public string getRoomName()
    {
        return roomName;
    }
}