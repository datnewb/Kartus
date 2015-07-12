using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    private MainMenuHandler mainMenuHandler;
    internal List<PlayerInfo> playerInfos;

    internal bool leftGame;

    void Start()
    {
        mainMenuHandler = GetComponent<MainMenuHandler>();
    }

    void OnEnable()
    {
        playerInfos = new List<PlayerInfo>();

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
            playerInfos.Add(playerInfo);

        if (Network.isServer)
        {
            playerInfos.Sort((x, y) => x.queueNumber.CompareTo(y.queueNumber));
            int currentQueueNumber = 0;
            foreach (PlayerInfo playerInfo in playerInfos)
                playerInfo.queueNumber = currentQueueNumber++;
        }
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
