using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameContoller : MonoBehaviour
{
    public Camera cam;
    private GameObject gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.tag == "Frame")
                {
                    GetComponent<FileBrowser>().OpenBrowser();
                }
            }
        }       
    }
}
