using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material[] mat = new Material[2];
    private bool is_create;
    public GameObject framePrefab;
    public GameObject frames;
    private Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider>();
        frames = GameObject.Find("Frames");
        GetComponent<MeshRenderer>().material = mat[0];
        is_create = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Frame")
        {
            GetComponent<MeshRenderer>().material = mat[1];
            is_create = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Frame")
        {
            GetComponent<MeshRenderer>().material = mat[0];
            is_create = true;
        }
    }

    public void AddFrame()
    {
        if (is_create)      //생성 가능 상태인 경우.
        {
            GameObject instance = Instantiate(framePrefab);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - 1.3f, transform.position.z);
            instance.transform.position = pos;
            instance.transform.rotation = transform.rotation;
            //instance.transform.SetParent(frames.transform);
        }
    }
}