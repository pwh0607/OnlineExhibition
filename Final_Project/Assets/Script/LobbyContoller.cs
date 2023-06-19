using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyContoller : MonoBehaviour
{
    private GameObject lastView;         //���� �� - Back ��ư��
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

        //�� ����Ʈ ����.
    }
    public void OnClickMake()
    {
        inputRoomName.SetActive(true);
    }
    public void pushX()
    {
        inputRoomName.SetActive(false);
        //���̸� ����� ��ũ��Ʈ ����.
    }

    public void OnClickBack()
    {   //���� ��ư ��Ȱ��ȭ, ���� ��ư Ȱ��ȭ.
        currentView.SetActive(false);
        lastView.SetActive(true);
    }
}