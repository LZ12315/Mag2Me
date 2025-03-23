using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;



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
        [SerializeField] private PlayableDirector director;

        public void ExecuteTimeline(BuffManager manager, UnityAction action)
        {
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
        EventCenter.Instance.AddEventListener("Combo", Combo);
    }

    void Combo()
    {
        comboNum++;
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

    void ExcuteBuff(BuffSetting buff)
    {
        switch(buff.BuffInfo.valueBuff)
        {
            case ValueBuff.None:
                break;
            case ValueBuff.HealthUp:
                buff.ExecuteTimeline(this, () => HealthUp(buff.BuffInfo.buffValue));
                break;
            case ValueBuff.PowerUp:
                buff.ExecuteTimeline(this, () => PowerUp(buff.BuffInfo.buffValue));
                break;
        }

        switch (buff.BuffInfo.ultimateBuff)
        {
            case UltimateBuff.None:
                break;
            case UltimateBuff.ImediateDead:
                buff.ExecuteTimeline(this, () => InvokeImediateDead(buff.BuffInfo.buffDuration));
                break;
            case UltimateBuff.InfinityBullet:
                buff.ExecuteTimeline(this, () => InvokeInfinityBullet(buff.BuffInfo.buffDuration));
                break;
            case UltimateBuff.DividedBullet:
                break;
        }
    }

    void HealthUp(int value)
    {
        Player playerCharacter = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerCharacter.HealthChange(this, value);
    }

    void PowerUp(int value)
    {
        EquipHolder playerHolder = GameObject.FindWithTag("Player").GetComponent<EquipHolder>();
        playerHolder.PowerUp(this, value);
    }

    void InvokeImediateDead(float Duration)
    {
        StartCoroutine(ImediatelyDead(Duration));
    }

    IEnumerator ImediatelyDead(float duration)
    {
        GameObject[] allObjects_Before = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects_Before)
        {
            if (!obj.activeInHierarchy) continue;

            Enemy enemyCharacter = obj?.GetComponent<Enemy>();
            if (enemyCharacter != null)
                enemyCharacter.SetimediateDead(this, true);
        }

        yield return new WaitForSeconds(duration);

        GameObject[] allObjects_After = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects_After)
        {
            if (!obj.activeInHierarchy || !gameObject.activeSelf) continue;

            Enemy enemyCharacter = obj?.GetComponent<Enemy>();
            if (enemyCharacter != null)
                enemyCharacter.SetimediateDead(this, false);
        }
    }

    void InvokeInfinityBullet(float Duration)
    {
        StartCoroutine(InfinityBullet(Duration));
    }

    IEnumerator InfinityBullet(float duration)
    {
        MagSource playerSource = GameObject.FindWithTag("Player").GetComponent<MagSource>();
        MagSourceInfo magSourceInfo = playerSource.GetSourceInfo(this);

        int maxHoldNum = magSourceInfo.maxHoldNum;
        float snapDistance = magSourceInfo.snapDistance;
        magSourceInfo.maxHoldNum = 100;
        magSourceInfo.snapDistance = 100;

        yield return new WaitForSeconds(duration);

        magSourceInfo.maxHoldNum = maxHoldNum;
        magSourceInfo.snapDistance = snapDistance;
    }

}
