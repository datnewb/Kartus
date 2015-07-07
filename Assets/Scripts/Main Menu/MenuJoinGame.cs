using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuJoinGame : MonoBehaviour
{
    private MainMenuHandler mainMenuHandler;
    private Canvas joinGameCanvas;

    [SerializeField]
    private InputField ipAddressField;

    void Start()
    {
        OnEnable();
    }

    void OnEnable()
    {
        mainMenuHandler = GetComponent<MainMenuHandler>();
        joinGameCanvas = mainMenuHandler.joinGameCanvas;

        MainMenuHandler.DisableAllCanvases();
        joinGameCanvas.enabled = true;
    }

    public void JoinGame()
    {
        Network.Connect(ipAddressField.text, NetworkManager.portNumber);
        mainMenuHandler.ShowMessageDialog("CONNECTING", "Trying to connect to " + (ipAddressField.text == "" ? "127.0.0.1" : ipAddressField.text));
    }

    public void ConnectionFailedConfirm()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();
    }

    void OnConnectedToServer()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();

        enabled = false;
        GetComponent<LobbyManager>().enabled = true;
        GetComponent<MenuLobby>().enabled = true;
        Network.Instantiate(mainMenuHandler.playerInfoPrefab, Vector3.zero, Quaternion.identity, 0);
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        string title = "CONNECTION FAILED";
        string message = "Failed to connect to server. Either server doesn't exist or network has problems.";
        UnityAction okAction = ConnectionFailedConfirm;

        mainMenuHandler.ShowErrorDialog(title, message, okAction);
    }
}
