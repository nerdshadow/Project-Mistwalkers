using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{    
    int maxHealth { get; set; }    
    public int currentHealth { get; set; }
    public bool isDead { get; set; }
    void InitHealth();
    void ResetHealth();
    void Die();
}
