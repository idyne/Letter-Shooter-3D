using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinText : MonoBehaviour
{
    private Text text = null;
    private RectTransform rect = null;

    private void Awake()
    {
        text = GetComponent<Text>();
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void Show(int place, bool success)
    {
        text.text = (success ? "WIN!\n#" : "LOSE!\n#") + place.ToString();
        Show();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        rect.LeanMoveY(rect.anchoredPosition.y + 100, 1.2f);
        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.alphaText(GetComponent<RectTransform>(), 0, 0.2f).setOnComplete(() => { gameObject.SetActive(false); });
        });
    }
}
