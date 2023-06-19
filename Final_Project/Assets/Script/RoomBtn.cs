using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomBtn : MonoBehaviour
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
    /*
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " " + roomName + "입장!!");

        //해당 방씬으로 이동.
        PhotonNetwork.LoadLevel("RoomScene");
        base.OnJoinedRoom();
    }
    */
}