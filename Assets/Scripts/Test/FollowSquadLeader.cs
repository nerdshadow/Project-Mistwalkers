using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSquadLeader : MonoBehaviour
{
    [SerializeField]
    GameObject squadLeader;
    public bool smoothFollow = false;
    [SerializeField]
    float speed = 2f;
    public List<Transform> squadMemberPositions = new List<Transform>();
    private void Update()
    {
        if (squadLeader == null)
            return;
        if (smoothFollow == true)
            LerpFollow();
        else
            Follow();
    }
    private void Follow()
    {
        Vector3 newLoc = new Vector3(squadLeader.transform.position.x, 0f, squadLeader.transform.position.z);
        transform.position = newLoc;
    }
    void LerpFollow()
    {
        if (Vector3.Distance(this.transform.position, squadLeader.transform.position) > 0.1f)
            transform.position = Vector3.Lerp(transform.position, squadLeader.transform.position, Time.deltaTime * speed);
    }
}
