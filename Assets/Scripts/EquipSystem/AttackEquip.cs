using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class AttackEquip : Equip
{
    private Collider2D attackCollider;
 
    [Header("π•ª˜…Ë÷√")]
    [SerializeField] private int attackPower = 1;
    [SerializeField] private int attackNum = 1;

    Dictionary<EquipHolder, List<DefenceEquip>> attackTargets = new Dictionary<EquipHolder, List<DefenceEquip>>();

    protected override void Start()
    {
        base.Start();
        attackCollider = GetComponent<Collider2D>();
    }

    float counter = 0;
    private void Update()
    {
        if(equipHolder)
        {
            counter += Time.deltaTime;
            if (counter > 2)
            {
                AttackSearch();
                counter = 0;
            }
        }
    }

    public void AttackSearch()
    {
        Collider2D[] objectInTrigger = new Collider2D[20];
        ContactFilter2D filter = new ContactFilter2D();

        int objectNum = Physics2D.OverlapCollider(attackCollider, filter, objectInTrigger);
        for (int i = 0; i < objectNum; i++)
        {
            if (objectInTrigger[i]?.GetComponent<EquipHolder>() != null)
                AddTarget(objectInTrigger[i].GetComponent<EquipHolder>());

            if (objectInTrigger[i]?.GetComponent<DefenceEquip>() != null)
                AddDefence(objectInTrigger[i].GetComponent<DefenceEquip>());
        }

        ExcuteAttack();
    }

    void AddTarget(EquipHolder holder)
    {
        if (holder == equipHolder) return;
        if (attackTargets.ContainsKey(holder)) return;

        List<DefenceEquip> defenceEquips = new List<DefenceEquip> ();
        attackTargets.Add(holder,defenceEquips);
    }

    void AddDefence(DefenceEquip defence)
    {
        if(defence.EquipHolder == equipHolder) return;

        EquipHolder holder = defence.EquipHolder;
        if (attackTargets.ContainsKey(holder))
        {
            attackTargets[holder].Add(defence);
            return;
        }
        List<DefenceEquip> defenceEquips = new List<DefenceEquip>();
        defenceEquips.Add(defence);
        attackTargets.Add(holder, defenceEquips);
    }

    void ExcuteAttack()
    {
        foreach (var target in attackTargets)
        {
            foreach (var defence in target.Value)
                defence.GetDamage(this, attackPower);

            if (target.Value.Count < attackNum)
            {
                int damageValue = (attackNum - target.Value.Count) * attackPower;
                target.Key.GetDamage(this, damageValue);
            }
        }

        if(attackTargets.Count > 0)
            UseEquip();
        attackTargets.Clear();
    }

}
