using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEnemy :EnemyController
{
    private void Update()
    {
        ChaseTarget();
    }

    void ChaseTarget()
    {
        if(player == null) return;
        Vector2 moveDir = (Vector2)(player.position - transform.position);
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
            Magnet magnet = collisions[i]?.GetComponent<Player>();
            if (magnet == null || magnet.MagnetParent != null) continue;


        }

    }

}
