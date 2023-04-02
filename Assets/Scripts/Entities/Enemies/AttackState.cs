using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    [SerializeField] Animator animator;
    [SerializeField] EnemySlime enemySlime;

    [SerializeField] private AudioClip attackSound;

    [SerializeField] private float secondsBetweenAttacks = 1f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float knockBackDistance = 2f;

    private float currentAttackTimer;

    public override void OnEnter()
    {
        enemySlime.SetSpeed(EnemySlime.Speed.Stop);
        currentAttackTimer = 0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        currentAttackTimer += Time.deltaTime;

        enemySlime.SetMovement(FindObjectOfType<Player>().transform.position);

        if (currentAttackTimer >= secondsBetweenAttacks)
        {
            currentAttackTimer = 0f;
            Attack();
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        Singleton.Instance.SoundManager.Play(attackSound);

        float attackRadius = 1f;

        var rayCastHit = Physics2D.CircleCast(transform.position, attackRadius, enemySlime.LastMoveDirection, enemySlime.PlayerAttackDistance, enemySlime.AttackTargets);

        if (rayCastHit && rayCastHit.collider.gameObject.TryGetComponent(out IDamageTaker damageTaker))
        {
            damageTaker.TakeDamage(attackDamage, transform.position, knockBackDistance);
        }
    }
}
