using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeSlider : MonoBehaviour
{
    [SerializeField] RectMask2D mask = null;
    private RectTransform rect = null;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Show()
    {
        rect.LeanScale(Vector3.one, 0.1f);
    }

    public void Hide()
    {
        rect.LeanScale(Vector3.zero, 0.1f);
    }

    public float value
    {
        get
        {
            return Mathf.Clamp(1 - (mask.padding.w / rect.sizeDelta.y), 0, 1);
        }
        set
        {
            Vector4 padding = mask.padding;
            padding.w = Mathf.Clamp((1 - value), 0, 1) * rect.sizeDelta.y;
            mask.padding = padding;
        }
    }

}
