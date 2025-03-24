using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FleeEnemy : EnemyController
{
    [Header("æ‡¿Î…Ë÷√")]
    [SerializeField] private float startFleeDistance = 4f;
    [SerializeField] private float fleeDistance = 1.5f;
    [SerializeField] private float fleeDuration = 2f;

    List<PlayerCharacter> attackedObjects = new List<PlayerCharacter>();

    protected override void Start()
    {
        base.Start();
        canAct = true;
    }

    private void Update()
    {
        TryFlee();

        if (canAct)
            ChaseTarget();
    }

    void TryFlee()
    {
        if(player == null || !canAct) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= startFleeDistance)
            StartCoroutine(Flee());
    }

    IEnumerator Flee()
    {
        Vector2 forceDir = (Vector2)(transform.position - player.transform.position).normalized;
        if (physicalCharacter != null)
        {
            physicalCharacter.AddForce(forceDir, fleeDistance, fleeDuration);
            canAct = false;
        }

        yield return new WaitForSeconds(fleeDuration);

        canAct = true;
    }

    void ChaseTarget()
    {
        if (player == null) return;
        Vector2 moveDir = (Vector2)(player.position - transform.position).normalized;
        physicalCharacter.SetVelocity(moveDir, moveSpeed);
    }

}
