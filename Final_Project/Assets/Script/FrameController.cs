using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    private string objName;

    private void OnEnable()
    {
        objName = "Frame" + RoomManager.instance.get_FrameCnt();
        RoomManager.instance.add_Frame();
    }
    public string getObjName()
    {
        return objName;
    }
}
