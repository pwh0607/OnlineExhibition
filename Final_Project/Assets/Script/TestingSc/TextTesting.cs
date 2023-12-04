using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextTesting : MonoBehaviour
{
    public GameObject tt;
    // Start is called before the first frame update
    void Start()
    {
        tt.GetComponent<TextMeshProUGUI>().text = "889";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
