using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBeh : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField]
    SceneField baseSceneLevel;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void StartNewGame()
    {
        Debug.Log("Started new game");
        gameManager.GeneratePath();
        gameManager.LoadLevel(baseSceneLevel);
    }
    public void LoadGame()
    {
        Debug.Log("loaded old game");
    }
    public void ExitGame()
    {
        Debug.Log("exited game");
        gameManager.CloseGame();
    }
    public void OpenSettings()
    {
        Debug.Log("opened settings");
    }

    public void ChangeFullscreen(bool _b)
    {
        Debug.Log("changing fullscreen " + _b);
        Screen.fullScreen = _b;
    }
    public void ChangeMasterVolume(float _volume)
    {
        Debug.Log("changing master volume " + _volume);
    }
    public void ChangeBigHats(bool _b)
    {
        Debug.Log("changing big hats " + _b);    
    }
}
