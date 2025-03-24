using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour, IMagSourceControl
{
    protected MagSource magSource;
    protected EquipHolder equipHolder;
    protected PhysicalCharacter physicalCharacter;
    [SerializeField] protected Collider2D enemyCollider;

    [Header("µ–»À…Ë÷√")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float snapDuration = 0.5f;

    protected Transform player;

    protected void Awake()
    {
        equipHolder = GetComponent<EquipHolder>();
        magSource = GetComponent<MagSource>();
        physicalCharacter = GetComponent<PhysicalCharacter>();
        enemyCollider = GetComponent<Collider2D>();

        player = GameObject.FindWithTag("Player").transform;
    }

    protected void Start()
    {
        StartCoroutine(ArmEquip());
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
