using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoadGameplay : MonoBehaviour
{
    GameManager gameManager;
    AudioManager audioManager;
    private void OnEnable()
    {
        gameManager = GameManager.instance;
        audioManager = AudioManager.instance;
    }
    private void OnDisable()
    {
        gameManager = null;
        audioManager = null;
    }

    public void StartDriving()
    {
        Debug.Log("Started driving");
    }
}
