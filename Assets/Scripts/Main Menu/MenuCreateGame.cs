using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuCreateGame : MonoBehaviour
{
    private MainMenuHandler mainMenuHandler;
    private Canvas createGameCanvas;

    [SerializeField]
    internal Text gameModeTitle;
    [SerializeField]
    internal Text gameModeDesc;
    [SerializeField]
    private GameObject gameModeHandler;

    internal GameModeButton selectedGameMode;

    void Start()
    {
        OnEnable();
    }

    void OnEnable()
    {
        mainMenuHandler = GetComponent<MainMenuHandler>();
        createGameCanvas = mainMenuHandler.createGameCanvas;

        selectedGameMode = null;
        gameModeTitle.text = "";
        gameModeDesc.text = "";

        MainMenuHandler.DisableAllCanvases();
        createGameCanvas.enabled = true;
    }

    public void CreateGame()
    {
        if (selectedGameMode == null)
        {
            string title = "ERROR";
            string message = "Please select a game mode.";
            UnityAction okAction = GameModeNotSelectedOK;

            mainMenuHandler.ShowErrorDialog(title, message, okAction);
        }
        else
        {
            Network.InitializeServer(3, NetworkManager.portNumber, false);
        }
    }

    public void GameModeNotSelectedOK()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();
    }

    void OnServerInitialized()
    {
        enabled = false;
        GameObject gameModeHandlerInstance = Network.Instantiate(gameModeHandler, Vector3.zero, Quaternion.identity, 0) as GameObject;
        switch (selectedGameMode.title)
        {
            case "Team Deathmatch":
                gameModeHandlerInstance.GetComponent<GameModeHandler>().gameMode = GameMode.TeamDeathmatch;
                break;
            case "Deathmatch":
                gameModeHandlerInstance.GetComponent<GameModeHandler>().gameMode = GameMode.Deathmatch;
                break;
            case "Power Insurgent":
                gameModeHandlerInstance.GetComponent<GameModeHandler>().gameMode = GameMode.PowerInsurgent;
                break;
        }

        GetComponent<MenuLobby>().enabled = true;

        Network.Instantiate(mainMenuHandler.playerInfoPrefab, Vector3.zero, Quaternion.identity, 0);
    }
}
