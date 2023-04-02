using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity, IDamageTaker
{
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
