using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MovingMapBehavior : MonoBehaviour
{
    public bool isManagingRoad = true;
    public bool isDeletingRoad = true;
    public int roadLength = 50;
    public GameObject referenceObject = null;
    public Transform frontRoadChunkTrans = null;
    public float distanceToCreateRoad = 75f;
    public Transform backRoadChunkTrans = null;
    public float distanceToDeleteRoad = 75f;
    public List<GameObject> mapPieces = new List<GameObject>();
    public List<GameObject> currentMapPieces = new List<GameObject>();
    public float speed = 1.0f;
    public UnityEvent onRoadPieceSpawned = new UnityEvent();
    void Start()
    {
        //CreateMapPiece();
        InitialCreateMap();        
    }
    private void FixedUpdate()
    {
        if (isManagingRoad == false)
            return;
        CheckReference();
        //MovingMapPieces();
    }
    private void OnDestroy()
    {
        onRoadPieceSpawned.RemoveAllListeners();
    }
    void InitialCreateMap()
    {
        if (mapPieces == null)
            return;

        currentMapPieces.Add(Instantiate(mapPieces[0], new Vector3(0, 0, -roadLength), Quaternion.identity));
        backRoadChunkTrans = currentMapPieces[0].transform;
        currentMapPieces.Add(Instantiate(mapPieces[0], new Vector3(0, 0, 0), Quaternion.identity));
        currentMapPieces.Add(Instantiate(mapPieces[0], new Vector3(0, 0, roadLength), Quaternion.identity));
        frontRoadChunkTrans = currentMapPieces[2].transform;
    }
    void CheckReference()
    {
        if(referenceObject == null)
            return;
        ManageCreatingRoad();
        if(isDeletingRoad == true)
            ManageDeletingRoad();
    }
    void ManageCreatingRoad()
    {
        float distFromFrontPieceToRef = 0;
        distFromFrontPieceToRef = Vector3.Distance(frontRoadChunkTrans.position, referenceObject.transform.position);
        if (distFromFrontPieceToRef <= distanceToCreateRoad)
        {
            //add new road, delete last
            CreateNextRoadChunk();
        }
    }
    void CreateNextRoadChunk()
    {
        //create a map piece
        //find possible chunk
        //create chunk

        //--- test
        GameObject potRoadChunk = Instantiate(mapPieces[0], frontRoadChunkTrans.transform.position + new Vector3(0, 0, roadLength), Quaternion.identity);
        currentMapPieces.Add(potRoadChunk);
        frontRoadChunkTrans = potRoadChunk.transform;

        potRoadChunk = null;

        onRoadPieceSpawned.Invoke();
    }
    void ManageDeletingRoad()
    {
        float distanceFromBackPieceToRef = 0;
        distanceFromBackPieceToRef = Vector3.Distance(backRoadChunkTrans.position, referenceObject.transform.position);
        if (distanceFromBackPieceToRef >= distanceToDeleteRoad)
        {
            DeleteLastRoadChunk();    
        }
    }
    void DeleteLastRoadChunk()
    {
        Debug.Log("Deleting road");
        currentMapPieces.RemoveAt(0);
        Destroy(backRoadChunkTrans.gameObject);
        backRoadChunkTrans = currentMapPieces[0].transform;
    }
}
