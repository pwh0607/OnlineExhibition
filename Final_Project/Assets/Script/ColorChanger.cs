using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material[] mat = new Material[2];

    private Collider coll;
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider>();
        GetComponent<MeshRenderer>().material = mat[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Frame")
        {
            GetComponent<MeshRenderer>().material = mat[1];
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Frame")
        {
            GetComponent<MeshRenderer>().material = mat[0];
        }
    }
}
