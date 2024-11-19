using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadPositionsMovement : MonoBehaviour
{
    [SerializeField]
    GameObject squadLeader;
    public bool smoothFollow = false;
    [SerializeField]
    float lerpSpeed = 2f;
    [SerializeField]
    float currentSpeed;
    public float maxSpeed = 8f;
    public float acceleration = 0.1f;
    public List<Transform> squadMemberPositions = new List<Transform>();
    private void Update()
    {
        if (squadLeader == null)
            return;
        if (smoothFollow == true)
            LerpFollowLeader();
        else
        {
            //FollowLeader();
            FollowLeaderWithOffset();
        }
    }
    private void OnEnable()
    {
        maxSpeed = squadLeader.GetComponent<VehicleMovement>().currentVehicleStats.maxSpeed / 3.6f;
    }
    //private void FixedUpdate()
    //{
    //    MoveBySelf();
    //}
    private void FollowLeader()
    {
        Vector3 newLoc = new Vector3(0, 0f, squadLeader.transform.position.z);
        transform.position = newLoc;
    }
    void LerpFollowLeader()
    {
        if (Vector3.Distance(this.transform.position, squadLeader.transform.position) > 0.1f)
            transform.position = Vector3.Lerp(transform.position, squadLeader.transform.position, Time.deltaTime * lerpSpeed);
    }
    void FollowLeaderWithOffset()
    {
        Vector3 offsetVector = squadLeader.transform.position - transform.position;
        transform.position = new Vector3(0, 0, transform.position.z + offsetVector.z);
    }
    void MoveBySelf()
    {
        float dist = MathF.Abs(squadMemberPositions[0].position.z - squadLeader.transform.position.z);
        if (dist < 5f)
        {
            //if (currentSpeed <= maxSpeed)
                currentSpeed += acceleration;
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed -= acceleration;
            if(currentSpeed < 0)
                currentSpeed = 0;
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
        }
    }
}
