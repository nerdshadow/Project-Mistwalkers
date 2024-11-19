using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoadEventsManager : MonoBehaviour
{
    public List<ScriptableObject> roadEvents = new List<ScriptableObject>();
    public UnityEvent onEventStart = new UnityEvent();
    public UnityEvent onEventEnd = new UnityEvent();
    public bool eventIsGoing = false;
    int currentEventPointer = 0;
    ScriptableObject currentEventSO = null;
    [SerializeField]
    MovingMapBehavior movingMapBehavior;
    [SerializeField]
    int roadCount = 0;
    public int minimalCountForEvent = 5;
    [SerializeField]
    RectTransform roadUIRef;
    [SerializeField]
    RectTransform roadUITextWindowRef;
    private void Start()
    {
        CheckList();
    }
    private void OnEnable()
    {
        if (movingMapBehavior != null)
            movingMapBehavior.onRoadPieceSpawned.AddListener(DoOnRoadSpawn);
    }
    private void OnDisable()
    {
        if (movingMapBehavior != null)
            movingMapBehavior.onRoadPieceSpawned.RemoveListener(DoOnRoadSpawn);
    }
    void CheckList()
    {
        foreach (ScriptableObject posEvent in roadEvents.ToList())
        {
            if (!(posEvent is IRoadEvent))
            {
                Debug.LogWarning("Non event object in event list");
                roadEvents.Remove(posEvent);
            }
        }
    }
    public void TryStartEvent()
    {
        if(currentEventPointer >= roadEvents.Count)
            return;
        currentEventSO = roadEvents[currentEventPointer];

        ((IRoadEvent)currentEventSO).StartEvent();
        StartCoroutine(TestCounter());
        onEventStart.Invoke();
        eventIsGoing=true;
    }
    public void TryEndEvent()
    {
        ((IRoadEvent)currentEventSO).EndEvent();
        currentEventPointer++;
        onEventEnd.Invoke();
        eventIsGoing = false;
    }
    void DoOnRoadSpawn()
    {
        if (eventIsGoing == true)
            return;
        if (roadCount < 0)
            roadCount = 0;
        roadCount++;

        if (roadCount >= minimalCountForEvent)
        {
            TryStartEvent();
            roadCount = 0;
        }
    }
    
    IEnumerator TestCounter()
    {
        yield return new WaitForSeconds(5f);
        TryEndEvent();
    }
}
