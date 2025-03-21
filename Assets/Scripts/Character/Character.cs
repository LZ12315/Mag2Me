using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private MagAnimation magAnimation;

    [Header("½ÇÉ«ÊôÐÔ")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        magAnimation = GetComponentInChildren<MagAnimation>();
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        magAnimation.HitVFX(this);
        Debug.Log(gameObject.name + "'s health is : " + currentHealth);

        if(currentHealth <= 0)
            Dead();
    }

    void Dead()
    {
        magAnimation.DeadVFX(this);
    }

}
