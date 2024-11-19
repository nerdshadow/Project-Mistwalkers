using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatEvent_SO", menuName = "SO/Event_Combat")]
public class CombatEvent_SO : ScriptableObject, IRoadEvent
{
    public List<GameObject> eventEnemies = new List<GameObject>();
    public int eventLevel = 0;

    public void EndEvent()
    {
        Debug.Log("Combat event ended");
    }

    public void StartEvent()
    {
        Debug.Log("Combat event started");
    }
    void SpawnEnemy(Vector3 positionToSpawn, GameObject enemyToSpawn)
    {
        GameObject currentEnemy = Instantiate(enemyToSpawn, positionToSpawn, Quaternion.identity);
    }
}
