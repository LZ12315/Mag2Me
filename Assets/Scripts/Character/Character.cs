using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("½ÇÉ«ÊôÐÔ")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + "'s health is : " + currentHealth);
        if(currentHealth <= 0)
            Destroy(gameObject);
    }
}
