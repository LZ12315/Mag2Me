using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Equip: MonoBehaviour
{
    [SerializeField] protected Magnet magnet;
    [SerializeField] protected EquipHolder equipHolder;
    [SerializeField] protected PhysicalCharacter physicCharacter;
    [SerializeField] protected Collider2D equipCollider;
    [SerializeField] protected TrailRenderer trailRenderer;

    [Header("装备设置")]
    [SerializeField] protected int maxEndurance = 3;
    [SerializeField] protected int currentEndurance;
    [SerializeField] private float defencePower = 1;

    [Header("发射设置")]
    [SerializeField] protected int attackPower = 1;
    [SerializeField] protected int attackNum = 1;
    [SerializeField] protected bool serviceable;
    [SerializeField] protected bool isBullet;

    int attackCounter = 0;
    List<EquipHolder> holdersWUdi = new List<EquipHolder>();

    protected virtual void Start()
    {
        magnet = GetComponent<Magnet>();
        physicCharacter = GetComponent<PhysicalCharacter>();
        equipCollider = GetComponent<Collider2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        serviceable = true;
        isBullet = false;
        currentEndurance = maxEndurance;
        if (trailRenderer != null)
            trailRenderer.enabled = false;
    }

    private void Update()
    {
        if (isBullet)
            EquipAttack();
    }

    public void EquipArmed(EquipHolder holder)
    {
        equipHolder = holder;
        holdersWUdi.Clear();
        serviceable = true;
    }

    void EquipRelieve(EquipHolder holder)
    {
        if (magnet != null)
            magnet.MagnetRelease(this);
        if (equipHolder = holder)
        {
            holdersWUdi.Add(equipHolder);
            equipHolder = null;
        }
    }

    public bool IsServiceable()
    {
        if(currentEndurance <= 0)
            serviceable = false;
        return serviceable;
    }

    #region 装备攻击
    public void EquipDamage(EquipHolder holder)
    {
        currentEndurance--;
        if (currentEndurance <= 0)
        {
            serviceable = false;
        }
    }

    public void ShootEquip(EquipHolder holder, Vector2 dir, float speed)
    {
        EquipRelieve(holder);
        GetComponent<Collider2D>().isTrigger = true;
        isBullet = true;
        serviceable = false;

        if(trailRenderer != null)
            trailRenderer.enabled = true;
        physicCharacter.SetVelocity(dir, speed);
    }

    void EquipAttack()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        Collider2D[] collisions = new Collider2D[20];

        int overlapCount = equipCollider.OverlapCollider(contactFilter, collisions);
        for (int i = 0; i < overlapCount; i++)
        {
            if (collisions[i].gameObject == gameObject) continue;
            EquipHolder target = collisions[i]?.GetComponent<EquipHolder>();
            if (target == null || target == equipHolder || holdersWUdi.Contains(target)) continue;

            target.GetDamage(this, attackPower);
            attackCounter++;
            if (attackCounter >= attackNum)
            {
                //gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }
        }
    }

    #endregion

    #region 其他

    public EquipHolder EquipHolder => equipHolder;

    public bool Serviceable => serviceable;

    #endregion

}
