using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected MagAnimation magAnimation;
    [SerializeField] protected EquipHolder equipHolder;

    [Header("½ÇÉ«ÊôÐÔ")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected bool imediateDeath;

    protected void Start()
    {
        currentHealth = maxHealth;
        magAnimation = GetComponentInChildren<MagAnimation>();
        equipHolder = GetComponent<EquipHolder>();
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 || imediateDeath)
        {
            Dead();
            return;
        }
        magAnimation.HitVFX(this);
    }

    protected void Dead()
    {
        EventCenter.Instance.EventTrigger("Combo");
        magAnimation.DeadVFX(this);
        StartCoroutine(DestroyGameObject());
    }

    protected IEnumerator DestroyGameObject()
    {
        Component[] components = GetComponents<Component>();
        foreach (Component component in components)
        {
            Behaviour behaviour = component as Behaviour;
            if (behaviour != null)
                behaviour.enabled = false;
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    #region Buff

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

    #endregion

}
