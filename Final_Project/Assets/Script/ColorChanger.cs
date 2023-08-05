using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material[] mat = new Material[2];
    private bool is_create;
    public GameObject framePrefab;

    private Collider coll;
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider>();
        GetComponent<MeshRenderer>().material = mat[0];
        is_create = false;
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
            Debug.Log("액자 생성!222222222222222");
            GameObject instance = Instantiate(framePrefab);
            instance.transform.position = gameObject.transform.position;
        }
    }
}
