using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override void GetDamage(Transform attackObject, int damage)
    {
        base.GetDamage(attackObject, damage);
        EventCenter.Instance.EventTrigger("ÕÊº“ ‹…À");
    }

    protected override void Dead()
    {
        base.Dead();
        EventCenter.Instance.EventTrigger("ÕÊº“À¿Õˆ");
    }

    protected override void HealthChange(int value)
    {
        base.HealthChange(value);
        EventCenter.Instance.EventTrigger<int>(EventName.HealthUp.ToString(), currentHealth);
    }
}
