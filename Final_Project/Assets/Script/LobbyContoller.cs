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

    // Start is called before the first frame update
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

        //방 리스트 갱신.
    }
    public void OnClickMake()
    {
        inputRoomName.SetActive(true);
    }
    public void pushX()
    {
        inputRoomName.SetActive(false);
        //방이름 지우는 스크립트 생성.
    }

    public void OnClickBack()
    {   //현재 버튼 비활성화, 이전 버튼 활성화.
        currentView.SetActive(false);
        lastView.SetActive(true);
    }
}