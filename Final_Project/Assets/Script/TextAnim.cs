using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnim : MonoBehaviour
{
    public float moveSpeed;
    public float fadeSpeed;

    TextMeshPro text;
    Color alpha;
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        alpha = text.color;
        Invoke("DesText", 1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().transform.Translate(new Vector2(0, moveSpeed * Time.deltaTime));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * fadeSpeed);
        text.color = alpha;
    }

    void DesText()
    {
        Destroy(gameObject);
    }
}
