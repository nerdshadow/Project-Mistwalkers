using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControls : MonoBehaviour
{
    public VehicleMovement vehicleMovement;
    public float vertInput;
    public float horInput;
    private void FixedUpdate()
    {
        if (vehicleMovement == null)
            return;

        float v = Input.GetAxis("Vertical");
        vertInput = v;
        float h = Input.GetAxis("Horizontal");
        horInput = h;

        if (v > 0.4)
            vehicleMovement.Move(VehicleMovement.Movement.forward);
        else if (v < -0.4)
            vehicleMovement.Move(VehicleMovement.Movement.stop);
        else
            vehicleMovement.Move(VehicleMovement.Movement.release);

        if (h > 0.4)
            vehicleMovement.Turn(VehicleMovement.TurnDir.Right);
        else if(h < -0.4)
            vehicleMovement.Turn(VehicleMovement.TurnDir.Left);
        else
            vehicleMovement.Turn(VehicleMovement.TurnDir.NoTurn);

        vehicleMovement.ManageWheels();
    }
}
