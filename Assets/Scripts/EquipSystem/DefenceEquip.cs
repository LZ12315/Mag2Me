using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEquip : Equip
{
    [Header("·ÀÓùÉèÖÃ")]
    [SerializeField] private float defencePower = 1;

    public bool GetDamage(Equip equip, int damage)
    {
        if (!serviceable) return false;

        Debug.Log(gameObject.name + " -> " + "DefenceEquip : " + damage);
        //UseEquip();
        return true;
    }
}
