using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    private MagSource snapSource;
    private EquipHolder equipHolder;
    private PhysicalCharacter physicalCharacter;

    [Header("µ–»À…Ë÷√")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float snapDuration = 0.5f;
    [SerializeField] private float attackInterval = 0.5f;

    Transform player;

    private void Awake()
    {
        equipHolder = GetComponent<EquipHolder>();
        snapSource = GetComponent<MagSource>();
        physicalCharacter = GetComponent<PhysicalCharacter>();

        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        StartCoroutine(ArmEquip());
    }

    float attackCounter = 0;
    private void Update()
    {
        if (player == null) return;
        Vector2 moveDir = (Vector2)(player.position - transform.position);
        physicalCharacter.SetVelocity(moveDir, moveSpeed);

        attackCounter += Time.deltaTime;
        if(attackCounter >= attackInterval)
        {
            Attack();
            attackCounter = 0;
        }
    }

    IEnumerator ArmEquip()
    {
        snapSource.isSnap = true;
        yield return new WaitForSeconds(snapDuration);
        snapSource.isSnap = false;
    }

    void Attack()
    {
        equipHolder.Shoot(player.transform.position);
    }

}
