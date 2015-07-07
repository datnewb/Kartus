using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuMain : MonoBehaviour
{
    [SerializeField]
    private GameModeButton powerInsurgent;
    [SerializeField]
    private GameObject gameModeHandler;
    [SerializeField]
    public Text playerNameText;
    [SerializeField]
    private Text nameChangeText;
    internal string playerName;

    private MainMenuHandler mainMenuHandler;
    private Canvas mainMenuCanvas;

    void Start()
    {
        OnEnable();
        CheckPlayerName();
    }

    void OnEnable()
    {
        mainMenuHandler = GetComponent<MainMenuHandler>();
        mainMenuCanvas = mainMenuHandler.mainMenuCanvas;
        MainMenuHandler.DisableAllCanvases();
        mainMenuCanvas.enabled = true;
    }

    public void CreateGame()
    {
        Network.InitializeServer(3, NetworkManager.portNumber, false);
    }

    public void Tutorial()
    {
        FindObjectOfType<GameManager>().LoadLevel("Tutorial 1", 4);
    }

    private void CheckPlayerName()
    {
        if (PlayerPrefs.GetString("playerName", "") == "")
        {
            playerNameText.text = "";
            ChangeNameDialog();
            nameChangeText.enabled = false;
        }
        else
        {
            ApplyPlayerName(PlayerPrefs.GetString("playerName"));
        }
    }

    public void ChangeNameDialog()
    {
        string title = "PLAYER NAME";
        string message = "Type in your name.";
        UnityAction okAction = PlayerNameOK;

        mainMenuHandler.ShowTextInputDialog(title, message, okAction);
    }

    public void ChangeNameDialogNotReq()
    {
        string title = "PLAYER NAME";
        string message = "Type in your name.";
        UnityAction okAction = PlayerNameOK;
        UnityAction cancelAction = PlayerNameCancel;

        mainMenuHandler.ShowTextInputNotReqDialog(title, message, okAction, cancelAction);
    }

    public void PlayerNameOK()
    {
        if (mainMenuHandler.dialogInstance.GetComponent<TextInputBox>().inputField.text != "")
        {
            ApplyPlayerName(mainMenuHandler.dialogInstance.GetComponent<TextInputBox>().inputField.text);

            Destroy(mainMenuHandler.dialogInstance);
            MainMenuHandler.EnableInputReceive();
        }
    }

    public void PlayerNameCancel()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();
    }

    public void ApplyPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString("playerName", playerName);
        playerNameText.text = "Hello, " + playerName + "!";

        nameChangeText.enabled = true;
    }

    public void ExitButton()
    {
        string title = "EXIT";
        string message = "Are you sure you want to quit?";
        UnityAction yesAction = ExitConfirm;
        UnityAction noAction = ExitDecline;

        mainMenuHandler.ShowConfirmDialog(title, message, yesAction, noAction);
    }

    public void ExitConfirm()
    {
        Application.Quit();
    }

    public void ExitDecline()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();
    }

    public void GoToCreateGame()
    {
        enabled = false;
        GetComponent<MenuCreateGame>().enabled = true;
    }

    public void GoToJoinGame()
    {
        enabled = false;
        GetComponent<MenuJoinGame>().enabled = true;
    }

    void OnServerInitialized()
    {
        enabled = false;
        GameObject gameModeHandlerInstance = Network.Instantiate(gameModeHandler, Vector3.zero, Quaternion.identity, 0) as GameObject;
        gameModeHandlerInstance.GetComponent<GameModeHandler>().gameMode = GameMode.PowerInsurgent;
        GetComponent<MenuLobby>().enabled = true;
        Network.Instantiate(mainMenuHandler.playerInfoPrefab, Vector3.zero, Quaternion.identity, 0);
    }
}
