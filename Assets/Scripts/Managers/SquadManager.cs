using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public List<VehicleMovement> vehicleSquad = new List<VehicleMovement>();
    public VehicleMovement leaderVehicle;
    public GameObject targetPrefab;
    public Transform mainTargetToFollow = null;
    public List<Transform> squadMemsPositions = new List<Transform>();
    //TODO
    //Leave targets far away and adjust max speed while
    private void OnEnable()
    {
        mainTargetToFollow = Instantiate(targetPrefab.transform);
        ChangeTargetsToFollow();
    }
    private void FixedUpdate()
    {
        ManageSquadDistance();
    }
    void ChangeTargetsToFollow()
    {
        if(mainTargetToFollow == null)
            return;

        leaderVehicle.ChangeFollowTarget(mainTargetToFollow);
        leaderVehicle.followSquadPosition = false;
        mainTargetToFollow.GetComponent<MoveTarget>().reference = leaderVehicle.transform;
        mainTargetToFollow.GetComponent<MoveTarget>().moveByReference = true;
        mainTargetToFollow.transform.position = new Vector3(leaderVehicle.transform.position.x,
                                                                leaderVehicle.transform.position.y,
                                                                mainTargetToFollow.transform.position.z);
        for (int i = 0; i < vehicleSquad.Count; i++)
        {
            if (vehicleSquad[i].currentMoveTarget != null)
                continue;
            vehicleSquad[i].ChangeFollowTarget(squadMemsPositions[i]);
            vehicleSquad[i].followSquadPosition = true;
        }

        //foreach (VehicleMovement vehicleMovement in vehicleSquad)
        //{
        //    if(vehicleMovement.currentMoveTarget != null)
        //        continue;
        //    Transform addTarget = Instantiate(targetPrefab.transform);
        //    vehicleMovement.ChangeFollowTarget(addTarget);
        //    vehicleMovement.followSquadPosition = true;
        //    addTarget.GetComponent<MoveTarget>().reference = vehicleMovement.transform;
        //    addTarget.GetComponent<MoveTarget>().moveByReference = true;
        //    addTarget.transform.position = new Vector3(vehicleMovement.transform.position.x,
        //                                                        vehicleMovement.transform.position.y,
        //                                                        addTarget.transform.position.z);
        //}
    }
    void ManageSquadDistance()
    {
        foreach (VehicleMovement vehicleMovement in vehicleSquad)
        {
            if(vehicleMovement == leaderVehicle)
                continue;
            float distZ = leaderVehicle.transform.position.z - vehicleMovement.transform.position.z;
            //Debug.Log(vehicleMovement.gameObject.name + " is " + distZ +" away" );
            if (distZ > 10)
            { 
                //Unblock max lerpSpeed for vehicle
                
            }
        }
    }
    public void ChangeSquad(List<VehicleMovement> newSquad)
    {
        vehicleSquad = new List<VehicleMovement>();
        vehicleSquad.AddRange(newSquad);
    }
    public void ChangeSquadMember(int squadIndex, VehicleMovement newMember)
    {
        vehicleSquad[squadIndex] = newMember;
    }
    //public void ChangeSquadMember(VehicleMovement squadMember, VehicleMovement newMember)
    //{
    //    if(vehicleSquad.Contains(squadMember))
    //    int buffIndex = List<VehicleMovement>().FindIndex(vehicleSquad, x => x == squadMember);
    //}
    public void MoveSquad()
    {
        foreach (VehicleMovement vehicle in vehicleSquad)
        {
            vehicle.MoveVehicle();
        }
    }
    public void StopSquad()
    {
        foreach (VehicleMovement vehicle in vehicleSquad)
        {
            vehicle.StopVehicle();
        }
    }

}
