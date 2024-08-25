using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    bool managerIsCreatedThisScene = false;
    List<Level_SO> levels = new List<Level_SO>();
    bool gameIsPaused = false;
    [SerializeField]
    GameObject loadingScreen;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        managerIsCreatedThisScene = true;
        InitManager();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += InitManager;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= InitManager;
    }
    void InitManager()
    {
        Debug.Log("Manager was created " + managerIsCreatedThisScene);
    }
    void InitManager(Scene _scene, LoadSceneMode _mode)
    {
        Debug.Log("Manager was created " + managerIsCreatedThisScene);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    public void LoadLevel(SceneField scene)
    {

        StopAllCoroutines();
        //if (AudioManager.instance != null)
        //    AudioManager.instance.StopAllCoroutines();
        //if (InputManager.instance != null)
        //    InputManager.instance.StopAllCoroutines();
        //if (ObjectPoolManager.instance != null)
        //    ObjectPoolManager.instance.StopAllCoroutines();

        GameObject currentLoadingScreen;
        //if (playerRef != null)
        //    currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        //else
            currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        Slider currentSlider = currentLoadingScreen.GetComponentInChildren<Slider>();
        StartCoroutine(LoadLevelAsync(scene, currentSlider));
    }
    IEnumerator LoadLevelAsync(SceneField _scene, Slider _loadingSlider)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_scene);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            _loadingSlider.value = progress;
            yield return null;
        }
        if (loadOperation.isDone)
        {
            DoOnLoadingScene();
        }
    }

    public void LoadLevel(string sceneName)
    {
        StopAllCoroutines();
        //if (AudioManager.instance != null)
        //    AudioManager.instance.StopAllCoroutines();
        //if (InputManager.instance != null)
        //    InputManager.instance.StopAllCoroutines();
        //if (ObjectPoolManager.instance != null)
        //    ObjectPoolManager.instance.StopAllCoroutines();

        GameObject currentLoadingScreen;
        //if (playerRef != null)
        //    currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        //else
            currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        Slider currentSlider = currentLoadingScreen.GetComponentInChildren<Slider>();
        StartCoroutine(LoadLevelAsync(sceneName, currentSlider));
    }
    IEnumerator LoadLevelAsync(string _sceneName, Slider _loadingSlider)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_sceneName);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            _loadingSlider.value = progress;
            yield return null;
        }
        if (loadOperation.isDone)
        {
            DoOnLoadingScene();
        }
    }

    void DoOnLoadingScene()
    {
        Time.timeScale = 1f;
        //InputManager.ChangeControlsMappingToGameplay();
        //if (ObjectPoolManager.instance != null)
        //    ObjectPoolManager.instance.ClearPools();
        //AudioManager.instance.PlayMusicForced(menuMusic, true);

        Scene scene = SceneManager.GetActiveScene();
        foreach (Level_SO levelSO in levels)
        {
            Debug.Log(levelSO.levelScene.SceneName);
            Debug.Log(scene.name);
        }
    }

    public void PauseGame()
    {
        //pause game
        //Debug.Log("Pausinggame");
        gameIsPaused = true;
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        //resume game
        //Debug.Log("UnPausinggame");
        gameIsPaused = false;
        Time.timeScale = 1f;
    }
}
