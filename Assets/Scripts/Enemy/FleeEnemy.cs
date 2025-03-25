using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FleeEnemy : EnemyController
{
    [Header("æ‡¿Î…Ë÷√")]
    [SerializeField] private float startFleeDistance = 4f;
    [SerializeField] private float stopFleeDistance = 6f;
    [SerializeField] private float fleeDistance = 1.5f;

    Vector2 moveDir;
    List<PlayerCharacter> attackedObjects = new List<PlayerCharacter>();

    protected override void Start()
    {
        base.Start();
        canAct = true;
    }

    protected override void Update()
    {
        base.Update();
        PlayerColliderDetect();
        if (!canAct) return;

        ChaseTarget();
        TryFlee();
        Move();
    }

    void TryFlee()
    {
        if(player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= startFleeDistance || distance <= stopFleeDistance)
            moveDir = (Vector2)(transform.position - player.position).normalized;
    }

    void ChaseTarget()
    {
        if (player == null) return;
        moveDir = (Vector2)(player.position - transform.position).normalized;
    }

    private void Move()
    {
        physicalCharacter.SetVelocity(moveDir, moveSpeed);
    }

    void PlayerColliderDetect()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        Collider2D[] collisions = new Collider2D[20];
        int overlapCount = enemyCollider.OverlapCollider(contactFilter, collisions);

        for (int i = 0; i < overlapCount; i++)
        {
            PlayerCharacter playerCharacter = collisions[i]?.GetComponent<PlayerCharacter>();
            if (playerCharacter == null || attackedObjects.Contains(playerCharacter)) continue;

            Vector2 forceDir = (Vector2)(transform.position - playerCharacter.transform.position);
            if (physicalCharacter != null)
                physicalCharacter.AddForceImpluse(forceDir, 1.5f);
        }
    }

}
