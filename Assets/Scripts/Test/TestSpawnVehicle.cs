using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnVehicle : MonoBehaviour
{
    [SerializeField]
    GameObject vehicle;
    [SerializeField]
    Transform position;
    [ContextMenu("Spawn")]
    void SpawnVehicle()
    {
        Instantiate(vehicle, position);
    }
}
