using UnityEngine;
using UnityEngine.Events;

public class UIPauseMenu : MonoBehaviour 
{
    [SerializeField]
    private Canvas pauseCanvas;
    [SerializeField]
    private GameObject confirmDialogPrefab;
    private GameObject confirmDialogInstance;

    internal bool inPauseMenu;


    void Start()
    {
        confirmDialogInstance = null;
    }

    void Update()
    {
        if (inPauseMenu)
        {
            pauseCanvas.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            pauseCanvas.enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ResumeGame()
    {
        inPauseMenu = false;
    }

    public void Settings()
    {

    }

    public void Quit()
    {
        MainMenuHandler.DisableInputReceive();

        string title = "QUIT";
        string message = "Do you really want to quit?";
        if (Network.isServer)
            message += " Other players will immediately get disconnected.";
        UnityAction yesAction = () => 
        {
            FindObjectOfType<GameManager>().LoadLevel("Main Menu", 0);
            Network.Disconnect(200);
        };
        UnityAction noAction = () => 
        {
            Destroy(confirmDialogInstance);
            MainMenuHandler.EnableInputReceive();
        };

        confirmDialogInstance = Instantiate(confirmDialogPrefab);
        ConfirmBox confirmDialogInfo = confirmDialogInstance.GetComponent<ConfirmBox>();
        confirmDialogInfo.title.text = title;
        confirmDialogInfo.message.text = message;
        confirmDialogInfo.yesButton.onClick.AddListener(yesAction);
        confirmDialogInfo.noButton.onClick.AddListener(noAction);
    }
}
