using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    [SerializeField]
    float speed = 2f;
    private void Update()
    {
        if (target == null)
            return;
        if(Vector3.Distance(this.transform.position, target.transform.position) > 0.1f)
            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
    }
}
