using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseVocalTest : MonoBehaviour
{
    [SerializeField] AkEvent _akEvent;

    [SerializeField] AK.Wwise.Event A;
    [SerializeField] AK.Wwise.Event B;
    [SerializeField] AK.Wwise.Event C;

    [SerializeField] GameObject VocalManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TriggerEvent(A);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TriggerEvent(B);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TriggerEvent(C);
        }
    }

    private void TriggerEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent != null)
        {
            // 使用当前游戏对象作为发射体
            wwiseEvent.Post(VocalManager);
        }
        else
        {
            Debug.LogWarning("Wwise事件未分配！");
        }
    }
}
