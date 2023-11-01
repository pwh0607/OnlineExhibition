using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextAnim : MonoBehaviour
{
    float time;
    public float _fadeTime = 2f;

    void Update()
    {
        if (time < _fadeTime)
        {
            GetComponent<TextMeshProUGUI>().color = new Color(0, 148, 255, 1f - time / _fadeTime);          //4������ ����!
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        time += Time.deltaTime;
    }

    public void OnEnable()
    {
        Debug.Log("�ؽ�Ʈ Ȱ��ȭ!!");
        GetComponent<TextMeshProUGUI>().color = new Color(0, 148, 255);
    }
    public void OnDisable()
    {
        time = 0;
    }
}