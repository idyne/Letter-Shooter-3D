using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapText : MonoBehaviour
{
    private Text text = null;
    private RectTransform rect = null;

    private void Awake()
    {
        text = GetComponent<Text>();
        rect = GetComponent<RectTransform>();
        Disappear();
    }

    public LTDescr Appear()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));
        rect.anchorMin = Vector2.one * Random.Range(0.4f, 0.6f);
        rect.anchorMax = rect.anchorMin;
        rect.anchoredPosition = Vector2.zero;
        return rect.LeanScale(Vector3.one, 0.2f);
    }

    public void Disappear()
    {
        LeanTween.cancel(gameObject);
        rect.localScale = Vector3.zero;
    }

    public void PopUp()
    {
        Disappear();
        Appear();
    }
}
