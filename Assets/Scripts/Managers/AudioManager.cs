using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    public AudioMixer audioMixer;
    public AudioMixerGroup masterMixerGroup;
    public float masterV = 1f;
    public AudioMixerGroup musicMixerGroup;
    public float musicV = 1f;
    public AudioMixerGroup sfxMixerGroup;
    public float sfxV = 1f;
    public AudioMixerGroup voiceMixerGroup;
    public float voiceV = 1f;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitManager();
    }
    void InitManager()
    {

    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        masterV = volume;
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        musicV = volume;
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        sfxV = volume;
    }
    public void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
        voiceV = volume;
    }
}
