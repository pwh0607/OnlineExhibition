using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FrameContoller : MonoBehaviour
{
   // public Camera cam;
    public Camera cam;
    private GameObject roomManager;
    private void Start()
    {
        roomManager = GameObject.Find("RoomManager");
        cam = GameObject.Find("mainCam").GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        SetImg();
    }


    void SetImg()
    {
        if (PhotonNetwork.IsMasterClient)       //방장인 경우
        {
            if (Input.GetMouseButtonDown(0) && roomManager.GetComponent<RoomManager>().GetMode() == 0)      //기본 모드 이고...
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform.gameObject.tag == "Frame")
                    {
                        GetComponent<FileBrowser>().OnClickImageLoad();
                    }
                }
            }
        }
    }
}
