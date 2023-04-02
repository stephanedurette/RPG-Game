using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageTaker
{
    public void TakeDamage(int damage, Vector3 sourcePosition, float knockBackDistance);

}
