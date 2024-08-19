using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestApplyForceToRigidCar : MonoBehaviour
{
    public Rigidbody body;
    public bool applyF = false;
    public float forceToApply = 1000f;
    private void FixedUpdate()
    {
        if (body == null)
            return;

        if (applyF == true)
        {
            body.AddRelativeForce(body.transform.forward * forceToApply, ForceMode.Acceleration);
        }
    }
}
