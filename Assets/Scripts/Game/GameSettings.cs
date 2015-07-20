using UnityEngine;
using System.Collections;

public class GameSettings : MonoBehaviour 
{
    internal Resolution[] supportedResolutions;
    internal Resolution currentResolution;
    internal Resolution previousResolution;
    internal int currentResolutionIndex;
    internal int previousResolutionIndex;

    internal bool currentFS;
    internal bool previousFS;

    internal bool currentVsync;
    internal bool previousVsync;

    internal int currentTextureQuality;
    internal int previousTextureQuality;

    internal int currentAA;
    internal int previousAA;

    internal float currentMouseSensitivity;
    internal float previousMouseSensitivity;

    internal float currentAudioVolume;
    internal float previousAudioVolume;

    internal bool valueChanged;

    void Start()
    {
        supportedResolutions = Screen.resolutions;
        currentResolution = Screen.currentResolution;
        previousResolution = currentResolution;
        currentResolutionIndex = 0;
        for (int res = 0; res < supportedResolutions.Length; res++)
        {
            if (supportedResolutions[res].width == currentResolution.width &&
                supportedResolutions[res].height == currentResolution.height)
            {
                currentResolutionIndex = res;
                break;
            }
        }
        previousResolutionIndex = currentResolutionIndex;

        currentFS = Screen.fullScreen;
        previousFS = currentFS;

        currentVsync = QualitySettings.vSyncCount == 0;
        previousVsync = currentVsync;

        currentTextureQuality = QualitySettings.masterTextureLimit;
        previousTextureQuality = currentTextureQuality;

        currentAA = QualitySettings.antiAliasing;
        previousAA = currentAA;

        currentMouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 10.0f);
        previousMouseSensitivity = currentMouseSensitivity;

        currentAudioVolume = PlayerPrefs.GetFloat("audioVolume", 1.0f);
        previousAudioVolume = currentAudioVolume;

        valueChanged = false;
    }

    void Update()
    {
        if (!valueChanged)
        {
            if (currentResolution.width != previousResolution.width &&
                currentResolution.height != previousResolution.height)
            {
                valueChanged = true;
            }
            else if (currentFS != previousFS)
            {
                valueChanged = true;
            }
            else if (currentVsync != previousVsync)
            {
                valueChanged = true;
            }
            else if (currentTextureQuality != previousTextureQuality)
            {
                valueChanged = true;
            }
            else if (currentAA != previousAA)
            {
                valueChanged = true;
            }
            else if (currentMouseSensitivity != previousMouseSensitivity)
            {
                valueChanged = true;
            }
            else if (currentAudioVolume != previousAudioVolume)
            {
                valueChanged = true;
            }
        }
        else
        {
            if (currentResolution.width == previousResolution.width &&
                currentResolution.height == previousResolution.height)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }

            if (currentFS == previousFS)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }

            if (currentVsync == previousVsync)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }

            if (currentTextureQuality == previousTextureQuality)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }

            if (currentAA == previousAA)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }

            if (currentMouseSensitivity == previousMouseSensitivity)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }

            if (currentAudioVolume == previousAudioVolume)
                valueChanged = false;
            else
            {
                valueChanged = true;
                return;
            }
        }
    }
}
