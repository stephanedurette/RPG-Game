using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity, IDamageTaker
{

    private new void OnEnable()
    {
        base.OnEnable();
        InputManager.OnAttackPressed += OnAttackPressed;
    }

    private void OnAttackPressed(object sender, System.EventArgs e)
    {
        float attackRadius = 1f;
        float attackDistance = 2.5f;
        Attack(attackRadius, attackDistance);
    }

    private new void OnDisable()
    {
        base.OnDisable();
        InputManager.OnAttackPressed -= OnAttackPressed;

    }

    // Update is called once per frame
    private void Update()
    {
        var normalizedInputVector = InputManager.MoveVector.normalized;

        lastMoveDirection = normalizedInputVector == Vector2.zero ? lastMoveDirection : normalizedInputVector;

        animator.SetBool("IsMoving", normalizedInputVector != Vector2.zero);
        animator.SetFloat("X Motion", lastMoveDirection.x);
        animator.SetFloat("Y Motion", lastMoveDirection.y);

        rigidBody.velocity = normalizedInputVector * walkSpeed;
    }

    public override void TakeDamage(Vector3 sourcePosition, AttackAttributesSO attackAttributes)
    {
        health.ChangeHealth(-attackAttributes.damage);
        transform.position += (transform.position - sourcePosition).normalized * attackAttributes.knockBackVelocity;
    }

    internal override void OnHealthEmpty(object sender, EventArgs e)
    {
        FindObjectOfType<GameOverPanel>().TogglePanel(true);
        gameObject.SetActive(false);
    }
}
