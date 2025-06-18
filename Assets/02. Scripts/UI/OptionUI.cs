using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField]
    Slider volumeSlider;

    [SerializeField]
    TMP_Dropdown sceneMode;

    [SerializeField]
    TMP_Dropdown resolution;

    [SerializeField]
    AudioMixer audioMixer;

    FullScreenMode fullScreenMode;

    public void SetVolume()
    {
        float volume = volumeSlider.value;
        float dB = volume <= 0 ? -80f : Mathf.Log10(volume / 100f) * 20f;

        audioMixer.SetFloat("MasterVolume", dB);
    }

    public void SetScreenMode()
    {
        int newSceneMode = sceneMode.value;

        switch(newSceneMode)
        {
            case 0:
                fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        Screen.SetResolution(Screen.width, Screen.height, fullScreenMode);
    }

    public void SetResolution()
    {
        string[] dimensions = resolution.options[resolution.value].text.Split("*");

        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        Screen.SetResolution(width, height, fullScreenMode);
    }
}
