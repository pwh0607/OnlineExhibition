using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UITesting : MonoBehaviour
{
    public Button btn;
    public GameObject btnPrefab;
    public GameObject viewPort;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickbtn()
    {
        GameObject instance = Instantiate(btnPrefab);
        instance.transform.parent = viewPort.transform;
        instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
}
