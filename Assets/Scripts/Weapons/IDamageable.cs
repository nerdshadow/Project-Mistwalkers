using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void DoDamage(int _damage);
    void DoDamage(int _damage, Vector3 _dmgPos);
    void DoDamage(int _damage, Vector3 _dmgPos, Vector3 _dmgVectorForce);
}
