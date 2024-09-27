using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    Sand = 0,
    Stone = 1,
}

public class LevelManager : MonoBehaviour
{
    public MapType mapType = MapType.Sand;
    public List<GameObject> tiles = new List<GameObject>();
    //private void OnEnable()
    //{
    //    CreateLevelMap(mapType);
    //}
    public void CreateLevelMap(MapType _mapType)
    {
        switch (_mapType)
        {
            case MapType.Sand:
                Instantiate(tiles[0]);
                break;
            case MapType.Stone:
                Instantiate(tiles[1]);
                break;
            default:
                break;
        }
    }
}
