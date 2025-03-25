using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboSignal : MonoBehaviour
{
    [SerializeField] private RectTransform maskTrans;
    [SerializeField] private RectMask2D mask;
    [SerializeField] private TextMeshProUGUI comboNumOutline;
    [SerializeField] private TextMeshProUGUI comboNumFilter;

    [Header("ComboÉèÖÃ")]
    [SerializeField] private bool combo = false;
    [SerializeField] private float comboDuration;
    [SerializeField] private float fadeSpeed = 1;

    public bool Combo => combo;
    float comboCounter = 0;
    float comboPercent = 0;
    float maskOriginalWidth = 0;
    int comboNum;

    private void Start()
    {
        EventCenter.Instance.AddEventListener(EventName.Combo.ToString(), RefreshCounter);

        comboCounter = 0;
        comboPercent = 0;
        if (maskTrans != null)
            maskOriginalWidth = maskTrans.rect.width;
        SetMask(0);
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
        SetComboNum(++comboNum);
    }

    void SetMask(float percent)
    {
        float width = percent * maskOriginalWidth;
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);
    }

    void SetComboNum(int num)
    {
        comboNumFilter.text = num.ToString();
        comboNumOutline.text = num.ToString();
    }

}
