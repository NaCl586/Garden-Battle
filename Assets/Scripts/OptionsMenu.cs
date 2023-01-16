using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer soundMixer;
    public AudioMixer musicMixer;
    public void SetSoundVolume(float soundvolume)
    {
        soundMixer.SetFloat("SoundVolume",soundvolume);
    }
    public void SetMusicVolume(float musicvolume)
    {
        musicMixer.SetFloat("MusicVolume", musicvolume);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
