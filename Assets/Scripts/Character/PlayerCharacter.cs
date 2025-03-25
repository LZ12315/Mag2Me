using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    protected override void Dead()
    {
        base.Dead();
        Application.Quit();
    }

    protected override void HealthChange(int value)
    {
        base.HealthChange(value);
        EventCenter.Instance.EventTrigger<int>(EventName.HealthUp.ToString(), currentHealth);
    }
}
