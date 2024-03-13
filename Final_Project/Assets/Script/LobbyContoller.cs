using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyContoller : MonoBehaviour
{
    private GameObject lastView;         //이전 뷰 - Back 버튼용
    private GameObject currentView;

    public GameObject visitView;
    public GameObject lobbyView;
    public GameObject inputRoomName;

    public Button backBtn;
    public Button visitBtn;
    public Button makeBtn;

    void Start()
    {
        visitView.SetActive(false);
        lobbyView.SetActive(true);
        inputRoomName.SetActive(false);
    }

    public void OnClickVisit()
    {
        lastView = lobbyView;
        currentView = visitView;
        lastView.SetActive(false);
        currentView.SetActive(true);
    }
    public void OnClickMake()
    {
        inputRoomName.SetActive(true);
    }
    public void pushX()
    {
        inputRoomName.SetActive(false);
    }

    public void OnClickBack()
    {
        currentView.SetActive(false);
        lastView.SetActive(true);
    }
}