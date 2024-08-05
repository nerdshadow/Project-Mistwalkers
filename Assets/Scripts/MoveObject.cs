using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed = 1f;
    private void FixedUpdate()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
