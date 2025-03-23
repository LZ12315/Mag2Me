using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
        public string name;
        [SerializeField] private int comboNum;
        [SerializeField] private BuffInfo buffInfo;

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
        for (int i = 0; i < buffSettings.Count; i++)
        {
            if(comboNum >= buffSettings[i].ComboNum)
            {
                ExcuteBuff(buffSettings[i].BuffInfo);
                buffSettings.Remove(buffSettings[i]);
            }
        }
    }

    void ExcuteBuff(BuffInfo buff)
    {
        switch(buff.valueBuff)
        {
            case ValueBuff.None:
                break;
            case ValueBuff.HealthUp:
                HealthUp(buff.buffValue);
                break;
            case ValueBuff.PowerUp:
                PowerUp(buff.buffValue);
                break;
        }

        switch (buff.ultimateBuff)
        {
            case UltimateBuff.None:
                break;
            case UltimateBuff.ImediateDead:
                StartCoroutine(ImediatelyDead(buff.buffDuration));
                break;
            case UltimateBuff.InfinityBullet:
                StartCoroutine(UltimateBullet(buff.buffDuration));
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

    IEnumerator ImediatelyDead(float duration)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (!obj.activeInHierarchy) continue;

            Enemy enemyCharacter = obj?.GetComponent<Enemy>();
            if (enemyCharacter != null)
                enemyCharacter.SetimediateDead(this, true);
        }

        yield return new WaitForSeconds(duration);

        foreach (GameObject obj in allObjects)
        {
            if (!obj.activeInHierarchy) continue;

            Enemy enemyCharacter = obj?.GetComponent<Enemy>();
            if (enemyCharacter != null)
                enemyCharacter.SetimediateDead(this, false);
        }
    }

    IEnumerator UltimateBullet(float duration)
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
