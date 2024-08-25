using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBeh : MonoBehaviour
{
    public void StartNewGame()
    {
        Debug.Log("Started new game");
    }
    public void LoadGame()
    {
        Debug.Log("loaded old game");
    }
    public void ExitGame()
    {
        Debug.Log("exited game");
    }
    public void OpenSettings()
    {
        Debug.Log("opened settings");
    }
}
