using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextAnim : MonoBehaviour
{
    float time;
    public float _fadeTime = 1.5f;

    void Update()
    {
        if (time < _fadeTime)
        {
            Debug.Log(GetComponent<TextMesh>().text);
            //GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 1f - time / _fadeTime);          //4차식은 투명도!
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        time += Time.deltaTime;
    }

    public void OnEnable()
    {
        Debug.Log("텍스트 활성화!!");
        GetComponent<TextMeshPro>().color = Color.white;
    }
    public void OnDisable()
    {
        time = 0;
    }
}