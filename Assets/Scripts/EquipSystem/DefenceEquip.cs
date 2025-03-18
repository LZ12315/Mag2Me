using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEquip : Equip
{
    [Header("∑¿”˘…Ë÷√")]
    [SerializeField] private float defencePower = 1;

    public void GetDamage(Equip equip, int damage)
    {
        Debug.Log("DefenceEquip : " + damage);
        UseEquip();
    }
}
