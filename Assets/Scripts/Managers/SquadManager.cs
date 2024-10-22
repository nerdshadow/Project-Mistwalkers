using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public List<VehicleMovement> vehicleSquad = new List<VehicleMovement>();
    public VehicleMovement leaderVehicle;

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
