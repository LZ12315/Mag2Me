using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Equip: MonoBehaviour
{
    protected Magnet magnet;
    protected EquipHolder equipHolder;
    protected PhysicalCharacter physicCharacter;
    protected Collider2D equipCollider;
    protected TrailRenderer trailRenderer;

    [Header("装备设置")]
    [SerializeField] protected int maxEndurance =  1;
    [SerializeField] protected int currentEndurance;

    [Header("发射设置")]
    [SerializeField] protected int attackPower = 1;
    [SerializeField] protected int attackNum = 1;
    [SerializeField] protected bool isBullet;

    int attackCounter = 0;
    List<GameObject> targetsCanNotAttack = new List<GameObject>();

    protected virtual void Start()
    {
        magnet = GetComponent<Magnet>();
        physicCharacter = GetComponent<PhysicalCharacter>();
        equipCollider = GetComponent<Collider2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

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
        targetsCanNotAttack.Clear();
        currentEndurance = maxEndurance;
    }

    public void EquipRelieve(EquipHolder holder)
    {
        if (magnet != null)
        {
            magnet.MagnetRelease(this);
            StartCoroutine(magnet.MagnetBanned(this, 1f));
        }

        equipHolder.ReleaseEquip(this);
        targetsCanNotAttack.Add(equipHolder.gameObject);
        equipHolder = null;
    }

    void DropEquip()
    {
        float angle = Random.Range(0, Mathf.PI * 2);
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        physicCharacter.AddForceImpluse(dir, 2);
    }

    public void EquipDamage(EquipHolder holder)
    {
        currentEndurance--;
        if (currentEndurance <= 0)
        {
            EquipRelieve(equipHolder);
            DropEquip();
        }
    }

    IEnumerator EquipDestroy()
    {
        Component[] components = GetComponents<Component>();
        foreach (Component component in components)
        {
            Behaviour behaviour = component as Behaviour;
            if (behaviour != null)
                behaviour.enabled = false;
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    #region 装备攻击

    public void ShootEquip(EquipHolder holder, Vector2 dir, float speed)
    {
        EquipRelieve(holder);
        if (magnet != null)
            StartCoroutine(magnet.MagnetBanned(this));
        GetComponent<Collider2D>().isTrigger = true;
        isBullet = true;

        if (trailRenderer != null)
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
            Character target = collisions[i]?.GetComponent<Character>();
            if (target == null || targetsCanNotAttack.Contains(target.gameObject)) continue;

            target.GetDamage(transform, attackPower);
            attackCounter++;
            targetsCanNotAttack.Add(target.gameObject);
            if (attackCounter >= attackNum)
            {
                StartCoroutine(EquipDestroy());
                return;
            }
        }
    }

    #endregion

    #region 其他

    public EquipHolder EquipHolder => equipHolder;

    public bool IsServiceable()
    {
        return currentEndurance > 0;
    }

    #endregion

}
