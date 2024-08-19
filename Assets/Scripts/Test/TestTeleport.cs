using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestTeleport : MonoBehaviour
{
    public List<GameObject> objectsToTeleport = new List<GameObject>();
    public float teleportDist = 75;
    [ContextMenu("Teleport")]
    public void Teleport()
    {
        foreach (GameObject obj in objectsToTeleport)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z - teleportDist);
        }
    }
}
