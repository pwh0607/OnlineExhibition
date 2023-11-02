using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomBtn : MonoBehaviourPunCallbacks
{
    public GameObject LobbyManager;
    string roomName;
    private void Start()
    {
        roomName = transform.GetChild(0).GetComponent<Text>().text;
        LobbyManager = GameObject.Find("LobbyManager");
        Debug.Log("버튼의 방이름은 " + roomName);
    }
    public void OnClick()
    {
        LobbyManager.GetComponent<LobbyManager>().OnClickRoom(roomName);
        Debug.Log("버튼 클릭!");
    }
}