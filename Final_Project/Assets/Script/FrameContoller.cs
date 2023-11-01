using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FrameContoller : MonoBehaviour
{
    public Camera cam;
    private GameObject roomManager;
    private void Start()
    {
        roomManager = GameObject.Find("RoomManager");
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
    }
}