using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSFXTest : MonoBehaviour
{
    [SerializeField] AkEvent _akEvent;

    [SerializeField] AK.Wwise.Event A;
    [SerializeField] AK.Wwise.Event B;
    [SerializeField] AK.Wwise.Event C;
    [SerializeField] AK.Wwise.Event D;
    [SerializeField] AK.Wwise.Event E;

    [SerializeField] GameObject MusicManager;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerEvent(A);
            TriggerEvent(B);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriggerEvent(B);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerEvent(C);
        }
    }

    private void TriggerEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent != null)
        {
            // 使用当前游戏对象作为发射体
            wwiseEvent.Post(MusicManager);
        }
        else
        {
            Debug.LogWarning("Wwise事件未分配！");
        }
    }
}
