using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehavior : MonoBehaviour, IDamageable, IHealth
{
    [field: Header("Health")]
    [field: SerializeField]
    public int maxHealth { get; set; }
    [field: SerializeField]
    public int currentHealth { get; set; }
    public bool isDead { get;  set; }

    public void DoDamage(int _damage)
    {
        currentHealth -= _damage;
    }

    public void DoDamage(int _damage, Vector3 _dmgPos)
    {
        throw new System.NotImplementedException();
    }

    public void DoDamage(int _damage, Vector3 _dmgPos, Vector3 _dmgVectorForce)
    {
        throw new System.NotImplementedException();
    }
    public void Die()
    {
        Debug.Log("Base died!");
        // game over 
    }

    public void InitHealth()
    {
        //Change max health depending on base stats
        maxHealth = 20;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
