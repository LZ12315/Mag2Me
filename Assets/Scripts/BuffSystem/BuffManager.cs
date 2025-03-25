using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static BuffManager;

public class BuffManager : MonoBehaviour
{
    [Serializable]
    public class BuffInfo
    {
        public ValueBuff valueBuff;
        public UltimateBuff ultimateBuff;
        public int buffValue;
        public float buffDuration;
    }

    [Serializable]
    public class BuffSetting
    {
        [SerializeField] private int comboNum;
        [SerializeField] private BuffInfo buffInfo;
        [SerializeField] public List<string> buffEvents = new List<string>();
        [SerializeField] private PlayableDirector director;

        public void ExecuteTimeline(BuffManager manager, UnityAction action)
        {
            if(director == null)
            {
                action?.Invoke();
                return;
            }

            director.Play();
            manager.StartCoroutine(manager.WatchTimelineProgress(director, action));
        }

        public int ComboNum => comboNum;
        public BuffInfo BuffInfo => buffInfo;
    }

    [SerializeField] 
    private List<BuffSetting> buffSettings = new List<BuffSetting>();
    [SerializeField]
    private int comboNum = 0;

    private void Start()
    {
        EventCenter.Instance.AddEventListener<GameObject>(EventName.EnemyDead.ToString(), (value) => Combo());
    }

    void Combo()
    {
        comboNum++;
        EventCenter.Instance.EventTrigger(EventName.Combo.ToString());
        foreach (var buff in buffSettings)
        {
            if (comboNum == buff.ComboNum)
            {
                ExcuteBuff(buff);
            }
        }
    }

    public IEnumerator WatchTimelineProgress(PlayableDirector director, UnityAction action)
    {
        while (director.state == PlayState.Playing)
        {
            yield return null;
        }
        action?.Invoke();
    }

    void ExcuteBuff(BuffSetting buffSetting)
    {
        foreach (var buffEvent in buffSetting.buffEvents)
            EventCenter.Instance.EventTrigger(buffEvent);

        switch(buffSetting.BuffInfo.valueBuff)
        {
            case ValueBuff.None:
                break;
            case ValueBuff.HealthUp:
                buffSetting.ExecuteTimeline(this, () => HealthUp(buffSetting.BuffInfo.buffValue));
                break;
            case ValueBuff.PowerUp:
                buffSetting.ExecuteTimeline(this, () => PowerUp(buffSetting.BuffInfo.buffValue));
                break;
        }

        switch (buffSetting.BuffInfo.ultimateBuff)
        {
            case UltimateBuff.None:
                break;
            case UltimateBuff.ImediateDead:
                buffSetting.ExecuteTimeline(this, () => InvokeImediateDead(buffSetting.BuffInfo.buffDuration));
                break;
            case UltimateBuff.InfinityBullet:
                buffSetting.ExecuteTimeline(this, () => InvokeInfinityBullet(buffSetting.BuffInfo.buffDuration));
                break;
            case UltimateBuff.DividedBullet:
                break;
        }
    }

    void HealthUp(int value)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
        playerCharacter.HealthChangeBuff(this, value);
    }

    void PowerUp(int value)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        EquipHolder playerHolder = player.GetComponent<EquipHolder>();
        playerHolder.PowerUp(this, value);
    }

    void InvokeImediateDead(float Duration)
    {
        StartCoroutine(ImediatelyDead(Duration));
    }

    IEnumerator ImediatelyDead(float duration)
    {
        EventCenter.Instance.EventTrigger("特殊奖励开始");

        GameObject[] allObjects_Before = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects_Before)
        {
            if (!obj.activeInHierarchy) continue;

            EnemyCharacter enemyCharacter = obj?.GetComponent<EnemyCharacter>();
            if (enemyCharacter != null)
                enemyCharacter.SetimediateDead(this);
        }

        yield return new WaitForSeconds(duration);

        EventCenter.Instance.EventTrigger("特殊奖励结束");
    }

    void InvokeInfinityBullet(float Duration)
    {
        StartCoroutine(InfinityBullet(Duration));
    }

    IEnumerator InfinityBullet(float duration)
    {
        EventCenter.Instance.EventTrigger("特殊奖励开始");

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) yield break;

        MagSource playerSource = player.GetComponent<MagSource>();
        MagSourceInfo magSourceInfo = playerSource.GetSourceInfo(this);

        int maxHoldNum = magSourceInfo.maxHoldNum;
        float snapDistance = magSourceInfo.snapDistance;
        magSourceInfo.maxHoldNum = 100;
        magSourceInfo.snapDistance = 100;

        yield return new WaitForSeconds(duration);

        magSourceInfo.maxHoldNum = maxHoldNum;
        magSourceInfo.snapDistance = snapDistance;
        EventCenter.Instance.EventTrigger("特殊奖励结束");
    }

}
