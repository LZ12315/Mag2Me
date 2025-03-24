using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemy : EnemyController
{
    [Header("≥Â¥Ã…Ë÷√")]
    [SerializeField] private float dashInterval = 2f;
    [SerializeField] private float dashChargeDuration = 0.5f;
    [SerializeField] private float dashPower = 2f;

    float waitDashCounter = 0;
    List<PlayerCharacter> attackedObjects = new List<PlayerCharacter>();

    private void Update()
    {
        AttckDetect();

        if (!canAct) return;

        TryDash();
        ChaseTarget();
    }

    void TryDash()
    {
        if (player == null) return;

        waitDashCounter += Time.deltaTime;
        if(waitDashCounter >= dashInterval)
            StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        physicalCharacter.AddForce(Vector2.zero, 0, dashChargeDuration);
        canAct = false;

        yield return new WaitForSeconds(dashChargeDuration);

        Vector2 dashDir = (Vector2)(player.position - transform.position).normalized;
        physicalCharacter.AddForceImpluse(dashDir, dashPower);

        canAct = true;
        waitDashCounter = 0;
    }

    void ChaseTarget()
    {
        if (player == null) return;
        Vector2 moveDir = (Vector2)(player.position - transform.position).normalized;
        physicalCharacter.SetVelocity(moveDir, moveSpeed);
    }

    void AttckDetect()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        Collider2D[] collisions = new Collider2D[20];
        int overlapCount = enemyCollider.OverlapCollider(contactFilter, collisions);

        for (int i = 0; i < overlapCount; i++)
        {
            PlayerCharacter playerCharacter = collisions[i]?.GetComponent<PlayerCharacter>();
            if (playerCharacter == null || attackedObjects.Contains(playerCharacter)) continue;

            playerCharacter.GetDamage(transform, attackPower);
            attackedObjects.Add(playerCharacter);
            StartCoroutine(AttackInterval());

            Vector2 forceDir = (Vector2)(transform.position - playerCharacter.transform.position).normalized;
            if (physicalCharacter != null)
                physicalCharacter.AddForceImpluse(forceDir, 1.5f);
        }

        if (attackedObjects.Count > 0)
            StartCoroutine(RefeshAttackedObjects());
    }

    IEnumerator AttackInterval()
    {
        canAct = false;
        yield return new WaitForSeconds(attackInterval);
        canAct = true;
    }

    IEnumerator RefeshAttackedObjects()
    {
        yield return new WaitForSeconds(attackRefreshTime);
        attackedObjects.Clear();
    }
}
