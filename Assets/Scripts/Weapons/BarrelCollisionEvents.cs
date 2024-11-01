using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BarrelCollisionEvents : MonoBehaviour
{
    public UnityEvent<Collider> somethingTriggers = new UnityEvent<Collider>();
    private void OnEnable()
    {
        if(somethingTriggers == null)
            somethingTriggers = new UnityEvent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        somethingTriggers.Invoke(other);
    }
}
