using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    internal Canvas mainMenuCanvas;
    [SerializeField]
    internal Canvas createGameCanvas;
    [SerializeField]
    internal Canvas joinGameCanvas;
    [SerializeField]
    internal Canvas lobbyCanvas;
    [SerializeField]
    internal Canvas loadingScreenCanvas;

    [SerializeField]
    private GameObject messageDialog;
    [SerializeField]
    private GameObject confirmDialog;
    [SerializeField]
    private GameObject errorDialog;
    [SerializeField]
    private GameObject textInputDialog;
    [SerializeField]
    private GameObject textInputNotReqDialog;
    internal GameObject dialogInstance;

    [SerializeField]
    internal GameObject playerInfoPrefab;

    void Start()
    {
        GetComponent<LobbyManager>().enabled = false;
    }

    public static void DisableAllCanvases()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (!canvas.isRootCanvas)
                canvas.enabled = false;
        }
    }

    public static void EnableAllCanvases()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (!canvas.isRootCanvas)
                canvas.enabled = true;
        }
    }

    public static void DisableInputReceive()
    {
        foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster>())
        {
            raycaster.enabled = false;
        }
    }

    public static void EnableInputReceive()
    {
        foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster>())
        {
            raycaster.enabled = true;
        }
    }

    public void ShowMessageDialog(string title, string message)
    {
        Destroy(dialogInstance);
        DisableInputReceive();
        dialogInstance = Instantiate(messageDialog);

        MessageBox messageBoxInfo = dialogInstance.GetComponent<MessageBox>();
        messageBoxInfo.title.text = title;
        messageBoxInfo.message.text = message;
    }

    public void ShowConfirmDialog(string title, string message, UnityAction yesAction, UnityAction noAction)
    {
        Destroy(dialogInstance);
        DisableInputReceive();
        dialogInstance = Instantiate(confirmDialog);

        ConfirmBox confirmDialogInfo = dialogInstance.GetComponent<ConfirmBox>();
        confirmDialogInfo.title.text = title;
        confirmDialogInfo.message.text = message;
        confirmDialogInfo.yesButton.onClick.AddListener(yesAction);
        confirmDialogInfo.noButton.onClick.AddListener(noAction);
    }

    public void ShowErrorDialog(string title, string message, UnityAction okAction)
    {
        Destroy(dialogInstance);
        DisableInputReceive();
        dialogInstance = Instantiate(errorDialog);

        ErrorBox errorDialogInfo = dialogInstance.GetComponent<ErrorBox>();
        errorDialogInfo.title.text = title;
        errorDialogInfo.message.text = message;
        errorDialogInfo.okButton.onClick.AddListener(okAction);
    }

    public void ShowTextInputDialog(string title, string message, UnityAction okAction)
    {
        Destroy(dialogInstance);
        DisableInputReceive();
        dialogInstance = Instantiate(textInputDialog);

        TextInputBox textInputDialogInfo = dialogInstance.GetComponent<TextInputBox>();
        textInputDialogInfo.title.text = title;
        textInputDialogInfo.message.text = message;
        textInputDialogInfo.okButton.onClick.AddListener(okAction);
    }

    public void ShowTextInputNotReqDialog(string title, string message, UnityAction okAction, UnityAction cancelAction)
    {
        Destroy(dialogInstance);
        DisableInputReceive();
        dialogInstance = Instantiate(textInputNotReqDialog);

        TextInputNotReqBox textInputDialogInfo = dialogInstance.GetComponent<TextInputNotReqBox>();
        textInputDialogInfo.title.text = title;
        textInputDialogInfo.message.text = message;
        textInputDialogInfo.okButton.onClick.AddListener(okAction);
        textInputDialogInfo.cancelButton.onClick.AddListener(cancelAction);
    }

    public void GoToMainMenu()
    {
        foreach (MonoBehaviour menuScript in gameObject.GetComponents<MonoBehaviour>())
        {
            if (menuScript.GetType() != typeof(GraphicRaycaster) &&
                menuScript.GetType() != typeof(CanvasScaler))
            menuScript.enabled = false;
        }
        enabled = true;
        GetComponent<MenuMain>().enabled = true;
    }
}
