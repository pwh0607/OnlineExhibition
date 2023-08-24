using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameContoller : MonoBehaviour
{
    public Camera cam;
    private GameObject roomManager;
    private void Start()
    {
        roomManager = GameObject.Find("RoomManager");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&& roomManager.GetComponent<RoomManager>().GetMode() == 1)
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
