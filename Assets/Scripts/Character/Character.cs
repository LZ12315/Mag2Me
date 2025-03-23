using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected MagAnimation magAnimation;

    [Header("½ÇÉ«ÊôÐÔ")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int currentHealth;
    private bool imediateDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        magAnimation = GetComponentInChildren<MagAnimation>();
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        magAnimation.HitVFX(this);

        //Debug.Log(gameObject.name + "'s health is : " + currentHealth);

        if(currentHealth <= 0)
            Dead();
    }

    void Dead()
    {
        EventCenter.Instance.EventTrigger("Combo");
        magAnimation.DeadVFX(this);
    }

    public void SetimediateDead(BuffManager manager, bool isTrue)
    {
        imediateDeath = isTrue;
    }

    public void HealthChange(BuffManager manager, int value)
    {
        currentHealth += value;
        if(currentHealth > maxHealth)
            maxHealth = currentHealth;
    }

}
