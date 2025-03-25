using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Animator aniGameState;
    [SerializeField] Animator aniPrice;

    private void Start()
    {
        EventCenter.Instance.AddEventListener("战斗开始动画", () => StartBattle());
        EventCenter.Instance.AddEventListener("休息阶段动画", () => StartRest());

        EventCenter.Instance.AddEventListener("特殊奖励开始",()=>StartPrice());
        EventCenter.Instance.AddEventListener("特殊奖励结束", () => EndPrice());
    }


    private void StartBattle()
    {
        aniGameState.SetTrigger("startBattle");
    }

    private void StartRest()
    {
        aniGameState.SetTrigger("startRest");
    }

    private void StartPrice()
    {
        aniPrice.SetBool("priceStart", true);
    }

    private void EndPrice()
    {
        aniPrice.SetBool("priceStart", false);
    }
}
