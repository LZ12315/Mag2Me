using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected MagAnimation magAnimation;
    protected EquipHolder equipHolder;
    protected PhysicalCharacter physicalCharacter;

    [Header("½ÇÉ«ÊôÐÔ")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected bool imediateDeath;

    protected void Start()
    {
        currentHealth = maxHealth;
        magAnimation = GetComponentInChildren<MagAnimation>();
        equipHolder = GetComponent<EquipHolder>();
        physicalCharacter = GetComponent<PhysicalCharacter>();
    }

    protected virtual void HealthChange(int value)
    {
        currentHealth += value;
        if(maxHealth < currentHealth)
            maxHealth = currentHealth;

        if (currentHealth <= 0)
            Dead();
    }

    public virtual void GetDamage(Transform attackObject, int damage)
    {
        Vector2 forceDir = (Vector2)(transform.position - attackObject.position);
        if (physicalCharacter != null)
            physicalCharacter.AddForceImpluse(forceDir, 1f);
        magAnimation.HitVFX(this);

        if (imediateDeath)
            Dead();

        int finalDamage = equipHolder.HolderDefence(damage);
        HealthChange(-finalDamage);
    }

    protected virtual void Dead()
    {
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

    public void SetimediateDead(BuffManager manager)
    {
        imediateDeath = true;
    }

    public virtual void HealthChangeBuff(BuffManager manager, int value)
    {
        HealthChange(value);
    }

    #endregion

}
