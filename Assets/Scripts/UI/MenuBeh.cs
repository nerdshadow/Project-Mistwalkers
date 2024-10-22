using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuBeh : MonoBehaviour
{
    public RectTransform settingsPanel;
    [Header("Managers")]
    public GameManager gameManager;
    public  AudioManager audioManager;
    [Space(10)]
    [Header("Levels")]
    [SerializeField]
    SceneField tutorialSceneLevel;
    private void Awake()
    {

    }
    private void Start()
    {
      
    }
    private void OnEnable()
    {
        gameManager = GameManager.instance;
        audioManager = AudioManager.instance;
        LoadSettings();
    }

    #region GameStates
    [Space(10)]
    [Header("Game")]
    public RectTransform newGameWindow;
    [SerializeField]
    public PlayerSaveData defaultSave = new PlayerSaveData();
    public void ToggleStartNewGameWindow()
    {
        if (newGameWindow.gameObject.activeInHierarchy == true)
        {
            newGameWindow.gameObject.SetActive(false);
        }
        else
        {
            newGameWindow.gameObject.SetActive(true);
            rSeed = UnityEngine.Random.Range(451, 9999999);
            randSeedInput.text = rSeed.ToString();
            //randSeedInput.onValueChanged.Invoke(randSeedInput.text);

            playerNameInput.text = pName;
        }
    }
    public void StartNewGame()
    {
        Debug.Log("Started new game");
        UnityEngine.Random.InitState(rSeed);
        //Create new savefile
        defaultSave.playerName = pName;
        defaultSave.playerRandSeed = rSeed;
        defaultSave.pathPoints = PathGenerator.GeneratePath(rSeed);
        PlayerSaveData startSave = defaultSave;
        RuntimePlayerSaveData.Instance.currentPlayerSaveData = startSave;
        SaveLoadSystem.SavePlayerData(startSave);

        gameManager.GeneratePath();
        gameManager.currentPathPoint = RuntimePlayerSaveData.Instance.currentPlayerSaveData.pathPoints[0];
        gameManager.levelType = LevelType.Road;
        Debug.Log("Loading start level");
        gameManager.LoadLevel(tutorialSceneLevel);
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
    public TMP_InputField randSeedInput;
    [SerializeField]
    int rSeed = 1;
    public void ChangeSeed(string _rSeed)
    {
        Int32.TryParse(_rSeed, out rSeed);
    }
    public TMP_InputField playerNameInput;
    [SerializeField]
    string pName = "Player";
    int sizeOfName = 12;
    public void ChangePName(string _pName)
    {
        pName = new string(_pName.Take(sizeOfName).ToArray());
    }
    #endregion GameStates
    #region Settings
    public void SaveSettings()
    {
        SettingsSaveData buffSettings = new SettingsSaveData(Screen.fullScreen, audioManager.masterV, audioManager.sfxV, audioManager.musicV, audioManager.voiceV);
        SaveLoadSystem.SaveSettingsData(buffSettings);
    }
    public void LoadSettings()
    {
        SaveLoadSystem.LoadSeetingsData();
        RefreshAudioSliders();
    }
    public void ResetSettings()
    {
        SaveLoadSystem.ResetSettingsData();
        RefreshAudioSliders();
    }
    public void ToggleSettings()
    {
        Debug.Log("opened settings");
        if (settingsPanel.gameObject.activeInHierarchy == true)
        {
            settingsPanel.gameObject.SetActive(false);
        }
        else
        {
            settingsPanel.gameObject.SetActive(true);
        }
    }
    public void ChangeFullscreen(bool _b)
    {
        Debug.Log("changing fullscreen " + _b);
        Screen.fullScreen = _b;
    }
    #region Audio
    [Space(10)]
    [Header("Audio")]
    public RectTransform audioPanel;
    public AudioSource testSource;
    public List<AudioClip> masterclips;
    public List<AudioClip> sfxclips;
    public List<AudioClip> musicclips;
    public List<AudioClip> voiceclips;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider voiceSlider;
    public void ToggleAudioPanel()
    {
        if (audioPanel.gameObject.activeInHierarchy == true)
        {
            audioPanel.gameObject.SetActive(false);
        }
        else
        {
            audioPanel.gameObject.SetActive(true);
        }
    }
    void RefreshAudioSliders()
    {
        masterSlider.value = audioManager.masterV;
        sfxSlider.value = audioManager.sfxV;
        musicSlider.value = audioManager.musicV;
        voiceSlider.value = audioManager.voiceV;
    }
    public void ChangeMasterVolume(float _volume)
    {
        audioManager.SetMasterVolume(_volume);
    }
    public void TestMasterVolume()
    {
        //play clip
        testSource.outputAudioMixerGroup = audioManager.masterMixerGroup;
        testSource.clip = masterclips[0];
        //testSource.volume = 1f;
        testSource.Play();
    }
    public void ChangeSFXVolume(float _volume)
    {
        audioManager.SetSFXVolume(_volume);
    }
    public void TestSFXVolume()
    {
        //play clip
        testSource.outputAudioMixerGroup = audioManager.sfxMixerGroup;
        testSource.clip = sfxclips[0];
        //testSource.volume = 1f;
        testSource.Play();
    }
    public void ChangeMusicVolume(float _volume)
    {
        audioManager.SetMusicVolume(_volume);
    }
    public void TestMusicVolume()
    {
        //play clip
        testSource.outputAudioMixerGroup = audioManager.musicMixerGroup;
        testSource.clip = musicclips[0];
        //testSource.volume = 1f;
        testSource.Play();
    }
    public void ChangeVoiceVolume(float _volume)
    {
        audioManager.SetVoiceVolume(_volume);
    }
    public void TestVoiceVolume()
    {
        //play clip
        testSource.outputAudioMixerGroup = audioManager.voiceMixerGroup;
        testSource.clip = voiceclips[0];
        //testSource.volume = 1f;
        testSource.Play();
    }
    #endregion Audio
    public void ChangeBigHats(bool _b)
    {
        Debug.Log("changing big hats " + _b);    
    }
    #endregion Settings
    #region Dev
    #endregion Dev
}
