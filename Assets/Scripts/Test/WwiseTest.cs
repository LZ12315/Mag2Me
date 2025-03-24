using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using static AkWwiseProjectData;
using static MoreMountains.Feedbacks.MMMiniPoolableObject;
using static MoreMountains.Tools.MMPoolableObject;


public class WwiseTest : MonoBehaviour
{
    [SerializeField] AkEvent _akEvent;

    [SerializeField] AK.Wwise.Event MainMenu;
    [SerializeField] AK.Wwise.Event SwitchToCombat;
    [SerializeField] AK.Wwise.Event SwitchToRest;
    [SerializeField] AK.Wwise.Event SwitchToBoss;
    [SerializeField] AK.Wwise.Event GameOver;

    [SerializeField] GameObject MusicManager;

    private void Start()
    {
        TriggerEvent(MainMenu);
        EventCenter.Instance.AddEventListener("Combo", () => StartGame());
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


    private void StartGame()
    {
        TriggerEvent(SwitchToCombat);
    }
}
