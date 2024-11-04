using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public MapType mapType = MapType.Sand;
    public List<GameObject> tiles = new List<GameObject>();

    private void OnEnable()
    {
        CreateNextChunkOfRoad();
    }

    void CreateNextChunkOfRoad()
    {
        
    }
}
