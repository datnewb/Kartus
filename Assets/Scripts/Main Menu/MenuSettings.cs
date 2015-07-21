using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuSettings : MonoBehaviour 
{
    private GameSettings gameSettings;

    [SerializeField]
    private Text resolutionText;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Toggle vSyncToggle;
    [SerializeField]
    private Text textureQualityText;
    [SerializeField]
    private Text antiAliasingText;
    [SerializeField]
    private Toggle motionBlurToggle;
    [SerializeField]
    private Slider mouseSensitivitySlider;
    [SerializeField]
    private Text mouseSensitivityText;
    [SerializeField]
    private Slider audioVolumeSlider;
    [SerializeField]
    private Text audioVolumeText;

    [SerializeField]
    private GameObject applyButton;

    [SerializeField]
    internal Canvas settingsCanvas;
    [SerializeField]
    private GameObject confirmDialogBox;
    private MainMenuHandler mainMenuHandler;

    private GameObject dialogInstance;

    void OnEnable()
    {
        if (GetComponent<MainMenuHandler>() != null)
        {
            mainMenuHandler = GetComponent<MainMenuHandler>();
            MainMenuHandler.DisableAllCanvases();
            mainMenuHandler.settingsCanvas.enabled = true;
        }

        gameSettings = FindObjectOfType<GameSettings>();
        fullScreenToggle.isOn = gameSettings.currentFS;
        vSyncToggle.isOn = gameSettings.currentVsync;
        SetResolutionText();
        SetTextureQualityText();
        SetAntiAliasingText();
        motionBlurToggle.isOn = gameSettings.currentMotionBlur;

        audioVolumeSlider.value = gameSettings.currentAudioVolume;
        SetAudioVolumeText();
        mouseSensitivitySlider.value = gameSettings.currentMouseSensitivity;
        SetMouseSensitivityText();

        dialogInstance = null;
    }

    void Update()
    {
        if (gameSettings.valueChanged)
            applyButton.SetActive(true);
        else
            applyButton.SetActive(false);
    }

    public void DecreaseResolution()
    {
        gameSettings.currentResolutionIndex--;
        if (gameSettings.currentResolutionIndex < 0)
            gameSettings.currentResolutionIndex = gameSettings.supportedResolutions.Length - 1;
        gameSettings.currentResolution = gameSettings.supportedResolutions[gameSettings.currentResolutionIndex];
        SetResolutionText();
    }

    public void IncreaseResolution()
    {
        gameSettings.currentResolutionIndex++;
        if (gameSettings.currentResolutionIndex >= gameSettings.supportedResolutions.Length)
            gameSettings.currentResolutionIndex = 0;
        gameSettings.currentResolution = gameSettings.supportedResolutions[gameSettings.currentResolutionIndex];
        SetResolutionText();
    }

    public void FullScreenToggled()
    {
        gameSettings.currentFS = fullScreenToggle.isOn;
    }

    public void VsyncToggled()
    {
        gameSettings.currentVsync = vSyncToggle.isOn;
    }

    public void MotionBlurToggled()
    {
        gameSettings.currentMotionBlur = motionBlurToggle.isOn;
    }

    public void DecreaseTextureQuality()
    {
        gameSettings.currentTextureQuality++;
        if (gameSettings.currentTextureQuality > 2)
            gameSettings.currentTextureQuality = 0;
        SetTextureQualityText();
    }

    public void IncreaseTextureQuality()
    {
        gameSettings.currentTextureQuality--;
        if (gameSettings.currentTextureQuality < 0)
            gameSettings.currentTextureQuality = 2;
        SetTextureQualityText();
    }

    public void IncreaseAntiAliasing()
    {
        switch (gameSettings.currentAA)
        {
            case 0:
                gameSettings.currentAA = 2;
                break;
            case 2:
                gameSettings.currentAA = 4;
                break;
            case 4:
                gameSettings.currentAA = 8;
                break;
            case 8:
                gameSettings.currentAA = 0;
                break;
        }
        SetAntiAliasingText();
    }

    public void DecreaseAntiAliasing()
    {
        switch (gameSettings.currentAA)
        {
            case 0:
                gameSettings.currentAA = 8;
                break;
            case 2:
                gameSettings.currentAA = 0;
                break;
            case 4:
                gameSettings.currentAA = 2;
                break;
            case 8:
                gameSettings.currentAA = 4;
                break;
        }
        SetAntiAliasingText();
    }

    private void SetResolutionText()
    {
        resolutionText.text = gameSettings.currentResolution.width + "x" + gameSettings.currentResolution.height;
    }

    private void SetTextureQualityText()
    {
        switch (gameSettings.currentTextureQuality)
        {
            case 0:
                textureQualityText.text = "High";
                break;
            case 1:
                textureQualityText.text = "Medium";
                break;
            case 2:
                textureQualityText.text = "Low";
                break;
        }
    }

    private void SetAntiAliasingText()
    {
        if (gameSettings.currentAA > 0)
            antiAliasingText.text = "MSAA x" + gameSettings.currentAA;
        else
            antiAliasingText.text = "Off";
    }

    public void SetMouseSensitivityText()
    {
        mouseSensitivityText.text = Mathf.RoundToInt(mouseSensitivitySlider.value) + "";
        gameSettings.currentMouseSensitivity = mouseSensitivitySlider.value;
    }

    public void SetAudioVolumeText()
    {
        audioVolumeText.text = Mathf.RoundToInt(audioVolumeSlider.value * 100) + "";
        gameSettings.currentAudioVolume = audioVolumeSlider.value;
    }

    public void ApplySettings()
    {
        gameSettings.previousResolution = gameSettings.currentResolution;
        gameSettings.previousResolutionIndex = gameSettings.currentResolutionIndex;
        gameSettings.previousFS = gameSettings.currentFS;
        Screen.SetResolution(gameSettings.currentResolution.width, gameSettings.currentResolution.height, gameSettings.currentFS);

        gameSettings.previousVsync = gameSettings.currentVsync;
        QualitySettings.vSyncCount = gameSettings.currentVsync ? 1 : 0;

        gameSettings.previousTextureQuality = gameSettings.currentTextureQuality;
        QualitySettings.masterTextureLimit = gameSettings.currentTextureQuality;

        gameSettings.previousAA = gameSettings.currentAA;
        QualitySettings.antiAliasing = gameSettings.currentAA;

        gameSettings.previousMotionBlur = gameSettings.currentMotionBlur;
        PlayerPrefs.SetInt("motionBlur", gameSettings.previousMotionBlur ? 1 : 0);

        gameSettings.previousMouseSensitivity = gameSettings.currentMouseSensitivity;
        PlayerPrefs.SetFloat("mouseSensitivity", gameSettings.currentMouseSensitivity);

        gameSettings.previousAudioVolume = gameSettings.currentAudioVolume;
        PlayerPrefs.SetFloat("audioVolume", gameSettings.currentAudioVolume);
    }

    public void ResetSettings()
    {
        gameSettings.currentResolution = gameSettings.previousResolution;
        gameSettings.currentResolutionIndex = gameSettings.previousResolutionIndex;
        gameSettings.currentFS = gameSettings.previousFS;
        gameSettings.currentVsync = gameSettings.previousVsync;
        gameSettings.currentTextureQuality = gameSettings.previousTextureQuality;
        gameSettings.currentAA = gameSettings.previousAA;
        gameSettings.currentMotionBlur = gameSettings.previousMotionBlur;
        gameSettings.currentMouseSensitivity = gameSettings.previousMouseSensitivity;
        gameSettings.currentAudioVolume = gameSettings.previousAudioVolume;
    }

    public void AcceptSettings()
    {
        ApplySettings();
        if (mainMenuHandler != null)
            mainMenuHandler.GoToMainMenu();
        else
            settingsCanvas.enabled = false;
    }

    public void BackToMainMenu()
    {
        if (gameSettings.valueChanged)
        {
            string title = "UNSAVED CHANGES";
            string message = "There are unsaved changes in the settings. Would you like to apply them first?";
            UnityAction yesAction = () =>
                {
                    ApplySettings();
                    mainMenuHandler.GoToMainMenu();
                    Destroy(mainMenuHandler.dialogInstance);
                    MainMenuHandler.EnableInputReceive();
                };
            UnityAction noAction = () =>
                {
                    ResetSettings();
                    mainMenuHandler.GoToMainMenu();
                    Destroy(mainMenuHandler.dialogInstance);
                    MainMenuHandler.EnableInputReceive();
                };

            mainMenuHandler.ShowConfirmDialog(title, message, yesAction, noAction);
        }
        else
            mainMenuHandler.GoToMainMenu();
    }

    public void BackToPauseMenu()
    {
        if (gameSettings.valueChanged)
        {
            string title = "UNSAVED CHANGES";
            string message = "There are unsaved changes in the settings. Would you like to apply them first?";
            UnityAction yesAction = () =>
            {
                ApplySettings();
                FindObjectOfType<UIPauseMenu>().pauseCanvas.enabled = true;
                settingsCanvas.enabled = false;
                Destroy(dialogInstance);
                dialogInstance = null;
                MainMenuHandler.EnableInputReceive();
            };
            UnityAction noAction = () =>
            {
                ResetSettings();
                FindObjectOfType<UIPauseMenu>().pauseCanvas.enabled = true;
                settingsCanvas.enabled = false;
                Destroy(dialogInstance);
                dialogInstance = null;
                MainMenuHandler.EnableInputReceive();
            };

            MainMenuHandler.DisableInputReceive();
            dialogInstance = Instantiate(confirmDialogBox);

            ConfirmBox confirmDialogInfo = dialogInstance.GetComponent<ConfirmBox>();
            confirmDialogInfo.title.text = title;
            confirmDialogInfo.message.text = message;
            confirmDialogInfo.yesButton.onClick.AddListener(yesAction);
            confirmDialogInfo.noButton.onClick.AddListener(noAction);
        }
        else
        {
            FindObjectOfType<UIPauseMenu>().pauseCanvas.enabled = true;
            settingsCanvas.enabled = false;
        }
    }
}
