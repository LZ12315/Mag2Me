using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboSignal : MonoBehaviour
{
    [SerializeField] private RectTransform maskTrans;
    [SerializeField] private RectMask2D mask;

    [Header("ComboÉèÖÃ")]
    [SerializeField] private bool combo = false;
    [SerializeField] private float comboDuration;
    [SerializeField] private float fadeSpeed = 1;

    public bool Combo => combo;
    float comboCounter = 0;
    float comboPercent = 0;
    float maskOriginalWidth = 0;

    private void Start()
    {
        EventCenter.Instance.AddEventListener("Combo", RefreshCounter);
        mask = GetComponentInChildren<RectMask2D>();
        maskTrans = mask.GetComponent<RectTransform>();

        comboCounter = 0;
        comboPercent = 0;
        if (maskTrans != null)
            maskOriginalWidth = maskTrans.rect.width;
    }

    private void Update()
    {
        if(!Combo) return;

        comboCounter -= Time.deltaTime * fadeSpeed;
        if(comboCounter > 0)
        {
            comboPercent = comboCounter / comboDuration;
            SetMask(comboPercent);
        }
        else
        {
            combo = false;
            comboCounter = 0;
            comboPercent = 0;
            SetMask(0);
        }
    }

    void RefreshCounter()
    {
        combo = true;
        comboCounter = comboDuration;
        comboPercent = 1;
        SetMask(1);
    }

    void SetMask(float percent)
    {
        float width = percent * maskOriginalWidth;
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);
    }

}
