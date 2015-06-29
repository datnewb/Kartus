using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    private MainMenuHandler mainMenuHandler;
    internal List<PlayerInfo> playerInfos;
    internal List<NetworkPlayer> players;

    internal bool leftGame;

    void Start()
    {
        mainMenuHandler = GetComponent<MainMenuHandler>();
    }

    void OnEnable()
    {
        playerInfos = new List<PlayerInfo>();
        players = new List<NetworkPlayer>();

        leftGame = false;
    }

    void Update()
    {
        UpdatePlayerInfoList();
    }

    private void UpdatePlayerInfoList()
    {
        playerInfos.Clear();
        foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            if (!playerInfos.Contains(playerInfo))
                playerInfos.Add(playerInfo);
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        players.Add(player);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.DestroyPlayerObjects(player);
        Network.RemoveRPCs(player);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        foreach (NetworkView networkView in FindObjectsOfType<NetworkView>())
        {
            if (networkView.stateSynchronization != NetworkStateSynchronization.Off)
                Destroy(networkView.gameObject);
        }
        if (Network.isClient)
        {
            if (info == NetworkDisconnection.Disconnected)
            {
                MainMenuHandler.DisableAllCanvases();
                string title = "SERVER CLOSED";
                string message = "The host left the game.";
                UnityAction okAction = () =>
                {
                    Destroy(mainMenuHandler.dialogInstance);
                    MainMenuHandler.EnableInputReceive();
                    mainMenuHandler.GoToMainMenu();
                };
                mainMenuHandler.ShowErrorDialog(title, message, okAction);
            }
        }
    }
}
