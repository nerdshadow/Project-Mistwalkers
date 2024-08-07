using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnVehicle : MonoBehaviour
{
    [SerializeField]
    GameObject vehicle;
    [SerializeField]
    Transform position;
    [SerializeField]
    TurretStats turretStats;
    [ContextMenu("SpawnVehicle")]
    void SpawnVehicle()
    {
        Instantiate(vehicle, position);
    }
    [ContextMenu("SpawnTurret")]
    void SpawnTurret()
    {
        Instantiate(turretStats.turretPrefab, position.transform.position, Quaternion.identity);
    }
}
