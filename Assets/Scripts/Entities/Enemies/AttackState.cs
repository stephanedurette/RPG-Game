using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    [SerializeField] Animator animator;
    [SerializeField] EnemySlime enemySlime;

    [SerializeField] private AudioClip attackSound;

    [SerializeField] private float secondsBetweenAttacks = 1f;

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

        if (Physics2D.CircleCast(transform.position, attackRadius, enemySlime.LastMoveDirection, enemySlime.PlayerAttackDistance, enemySlime.AttackTargets))
        {
            Debug.Log("hit");
        }
    }
}
