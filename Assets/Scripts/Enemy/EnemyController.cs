using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour, IMagSourceControl
{
    protected Collider2D enemyCollider;
    protected MagSource magSource;
    protected EquipHolder equipHolder;
    protected PhysicalCharacter physicalCharacter;

    [Header("µ–»À…Ë÷√")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float snapDuration = 0.5f;
    [SerializeField] protected float instantiateWaitTime = 1.5f;
    [SerializeField] protected bool canAct;

    [Header("π•ª˜ Ù–‘")]
    [SerializeField] protected int attackPower = 1;
    [SerializeField] protected float attackInterval = 0.25f;
    [SerializeField] protected float attackRefreshTime = 0.5f;

    protected Transform player;

    protected void Awake()
    {
        equipHolder = GetComponent<EquipHolder>();
        magSource = GetComponent<MagSource>();
        physicalCharacter = GetComponent<PhysicalCharacter>();
        enemyCollider = GetComponent<Collider2D>();

        player = GameObject.FindWithTag("Player").transform;
    }

    protected virtual void Start()
    {
        //StartCoroutine(ArmEquip());
        StartCoroutine(WaitToStart());
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(instantiateWaitTime);
        canAct = true;
    }

    IEnumerator ArmEquip()
    {
        magSource.ExcuteSnap(this);
        yield return new WaitForSeconds(snapDuration);
        magSource.SnapStop(this);
    }

    public void SnapObject(MagSource source)
    {
        if (source != magSource) return;
    }

}
