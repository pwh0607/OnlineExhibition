using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ColorChanger : MonoBehaviourPunCallbacks
{
    public Material[] mat = new Material[2];
    private bool is_create;
    public GameObject framePrefab;

    // Start is called before the first frame update
    void Start()
    {
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
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - 1.3f, transform.position.z);
            GameObject instance = PhotonNetwork.InstantiateRoomObject(framePrefab.name, pos, transform.rotation);
            int cnt = RoomManager.instance.get_FrameCnt();
            instance.name = "Frame" + cnt;
        }
    }
}