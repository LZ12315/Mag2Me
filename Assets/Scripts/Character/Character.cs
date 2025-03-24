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

    public void GetDamage(Transform attackObject, int damage)
    {
        int finalDamage = equipHolder.HolderDefence(damage);
        currentHealth -= finalDamage;

        Vector2 forceDir = (Vector2)(transform.position - attackObject.position);
        if (physicalCharacter != null)
            physicalCharacter.AddForceImpluse(forceDir, 1f);
        magAnimation.HitVFX(this);

        if (currentHealth <= 0 || imediateDeath)
            Dead();
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
