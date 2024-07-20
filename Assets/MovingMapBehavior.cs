using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingMapBehavior : MonoBehaviour
{
    public List<GameObject> mapPiece = new List<GameObject>();
    public List<GameObject> currentMapPieces = new List<GameObject>();
    public float speed = 1.0f;
    void Start()
    {
        //CreateMapPiece();
        InitialCreateMap();
    }
    private void FixedUpdate()
    {
        MovingMapPieces();   
    }
    void InitialCreateMap()
    {
        if (mapPiece == null)
            return;

        currentMapPieces.Add(Instantiate(mapPiece[0], new Vector3(0, 0, -25), Quaternion.identity));
        currentMapPieces.Add(Instantiate(mapPiece[0], new Vector3(0, 0, 0), Quaternion.identity));
        currentMapPieces.Add(Instantiate(mapPiece[0], new Vector3(0, 0, 25), Quaternion.identity));
    }
    void MovingMapPieces()
    {
        if (currentMapPieces.Count == 0)
            return;
        List<GameObject> pieceToDelete = new List<GameObject>();
        foreach (GameObject mapPiece in currentMapPieces) 
        {
            if (mapPiece == null)
                return;
            if (mapPiece.transform.position.z <= -35)
            {
                //DeleteMapPiece(mapPiece);
                pieceToDelete.Add(mapPiece);
            }
            mapPiece.transform.position += new Vector3(0, 0, -1 * speed);
        }
        foreach (GameObject mapPiece in pieceToDelete)
        {
            currentMapPieces.Remove(mapPiece);
            DeleteMapPiece(mapPiece);
            CreateNewMapPiece();
        }
    }
    void CreateNewMapPiece()
    {
        if (mapPiece == null)
            return;
        currentMapPieces.Add(Instantiate(mapPiece[0], currentMapPieces.Last().transform.position + new Vector3(0, 0, 25), Quaternion.identity));
    }
    void DeleteMapPiece(GameObject _mapPiece)
    {
        Destroy(_mapPiece);
    }
}
