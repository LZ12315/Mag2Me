using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEnemy :EnemyController
{
    List<PlayerCharacter> attackedObjects = new List<PlayerCharacter>();

    protected override void Update()
    {
        base.Update();
        AttckDetect();

        if (!canAct) return;
        ChaseTarget();
    }

    void ChaseTarget()
    {
        if(player == null) return;
   
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

            Vector2 forceDir = (Vector2)(transform.position - playerCharacter.transform.position);
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
