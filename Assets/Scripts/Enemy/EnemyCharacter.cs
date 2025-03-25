using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public override void GetDamage(Transform attackObject, int damage)
    {
        base.GetDamage(attackObject, damage);
        EventCenter.Instance.EventTrigger("µ–»À ‹…À");
    }

    protected override void Dead()
    {
        base.Dead();
        equipHolder.ReleaseEquipAll();
        EventCenter.Instance.EventTrigger<GameObject>(EventName.EnemyDead.ToString(), gameObject);
        EventCenter.Instance.EventTrigger("µ–»ÀÀ¿Õˆ");
    }
}
