using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    protected override void Dead()
    {
        base.Dead();
        equipHolder.ReleaseAllEquip();
        EventCenter.Instance.EventTrigger<GameObject>(EventName.EnemyDead.ToString(), gameObject);
    }
}
