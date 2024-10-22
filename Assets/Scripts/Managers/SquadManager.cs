using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public List<VehicleBehaviour> vehicleSquad = new List<VehicleBehaviour>();
    public VehicleBehaviour leaderVehicle;

    public void ChangeSquad(List<VehicleBehaviour> newSquad)
    {
        vehicleSquad = new List<VehicleBehaviour>();
        vehicleSquad.AddRange(newSquad);
    }

    public void ChangeSquadMember(int squadIndex, VehicleBehaviour newMember)
    {
        vehicleSquad[squadIndex] = newMember;
    }
    //public void ChangeSquadMember(VehicleBehaviour squadMember, VehicleBehaviour newMember)
    //{
    //    if(vehicleSquad.Contains(squadMember))
    //    int buffIndex = List<VehicleBehaviour>().FindIndex(vehicleSquad, x => x == squadMember);
    //}
    public void MoveSquad()
    {
        foreach (VehicleBehaviour vehicle in vehicleSquad)
        {
            vehicle.MoveVehicle();
        }
    }
    public void StopSquad()
    {
        foreach (VehicleBehaviour vehicle in vehicleSquad)
        {
            vehicle.StopVehicle();
        }
    }

}
