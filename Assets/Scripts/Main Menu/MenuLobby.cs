﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MenuLobby : MonoBehaviour
{
    [SerializeField]
    internal Text gameMode;
    [SerializeField]
    internal Text ipAddress;
    [SerializeField]
    internal GameObject startGameButton;

    [SerializeField]
    internal GameObject playerListWithTeam;
    [SerializeField]
    internal List<Text> playerListWithTeamText;

    [SerializeField]
    internal GameObject playerListNoTeam;
    [SerializeField]
    internal List<Text> playerListNoTeamText;

    [SerializeField]
    internal Text reservePlayersText;

    [SerializeField]
    internal Button speedsterButton;
    [SerializeField]
    internal Button roadkillButton;

    [SerializeField]
    internal Text countdownText;

    [SerializeField]
    internal GameObject reserveButton;

    private MainMenuHandler mainMenuHandler;
    private Canvas lobbyCanvas;
    private LobbyManager lobbyManager;

    internal bool leftGame;
    internal bool kartSelected;

    void Start()
    {
        OnEnable();
    }

    void OnEnable()
    {
        mainMenuHandler = GetComponent<MainMenuHandler>();
        lobbyCanvas = mainMenuHandler.lobbyCanvas;
        lobbyManager = GetComponent<LobbyManager>();

        MainMenuHandler.DisableAllCanvases();
        lobbyCanvas.enabled = true;
        lobbyManager.enabled = true;

        countdownText.text = "";
        reservePlayersText.text = "";
        ClearPlayerList(playerListNoTeamText);
        ClearPlayerList(playerListWithTeamText);

        SetGameMode();
        SetIPAddress();
        SetPlayerListView();

        if (Network.isServer)
            startGameButton.SetActive(true);
        else if (Network.isClient)
            startGameButton.SetActive(false);

        leftGame = false;
        kartSelected = false;
    }

    void Update()
    {
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().teams)
            {
                case GameModeTeams.None:
                    UpdatePlayerList(playerListNoTeamText, lobbyManager.playerInfos);
                    break;
                case GameModeTeams.Two:
                    UpdatePlayerListTeam(playerListWithTeamText, lobbyManager.playerInfos);
                    break;
            }
        }
        if (kartSelected)
        {
            PlayerInfo myPlayerInfo = null;
            foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
            {
                if (playerInfo.GetComponent<NetworkView>().isMine)
                {
                    myPlayerInfo = playerInfo;
                    break;
                }
            }
            myPlayerInfo.kartVariation = FindKartMatches(myPlayerInfo);
        }
        SetPlayerListView();
        SetPlayerTeam();
    }

    private void SetGameMode()
    {
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().gameMode)
            {
                case GameMode.Deathmatch:
                    gameMode.text = "Deathmatch";
                    reserveButton.SetActive(false);
                    break;
                case GameMode.PowerInsurgent:
                    gameMode.text = "Power Insurgent";
                    reserveButton.SetActive(true);
                    break;
                case GameMode.TeamDeathmatch:
                    gameMode.text = "Team Deathmatch";
                    reserveButton.SetActive(true);
                    break;
            }
        }
    }

    private void SetPlayerListView()
    {
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().teams)
            {
                case GameModeTeams.None:
                    playerListNoTeam.SetActive(true);
                    playerListWithTeam.SetActive(false);
                    break;
                case GameModeTeams.Two:
                    playerListWithTeam.SetActive(true);
                    playerListNoTeam.SetActive(false);
                    break;
            }
        }
    }

    private void SetIPAddress()
    {
        if (Network.isClient)
            ipAddress.text = Network.connections[0].ipAddress;
        else if (Network.isServer)
            ipAddress.text = Network.player.ipAddress;
    }

    private void SetPlayerTeam()
    {
        if (Network.isServer)
        {
            foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
            {
                if (playerInfo.position == 1 || playerInfo.position == 2)
                    playerInfo.GetComponent<CharacterTeam>().team = Team.Speedster;
                else if (playerInfo.position == 3 || playerInfo.position == 4)
                    playerInfo.GetComponent<CharacterTeam>().team = Team.Roadkill;
                else
                    playerInfo.GetComponent<CharacterTeam>().team = Team.NeutralHostile;
            }
        }
    }

    public void StartButton()
    {
        if (ReservesCount() > 0)
        {
            string title = "ERROR";
            string message = "Not all players are in a team. Wait for them to choose a team.";
            UnityAction okAction = () =>
            {
                MainMenuHandler.EnableInputReceive();
                Destroy(mainMenuHandler.dialogInstance);
            };

            mainMenuHandler.ShowErrorDialog(title, message, okAction);
        }
        else
        {
            MainMenuHandler.DisableInputReceive();
            StartCoroutine(CountDownToStart());
        }
    }

    IEnumerator CountDownToStart()
    {
        for (int currentTime = 1; currentTime >= 0; currentTime--)
        {
            GetComponent<NetworkView>().RPC("UpdateTimerText", RPCMode.All, "GAME STARTS IN " + currentTime);
            yield return new WaitForSeconds(1.0f);
        }

        string levelToLoad = "";
        int levelPrefix = 0;
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().gameMode)
            {
                case GameMode.TeamDeathmatch:
                    levelToLoad = "TeamDeathmatch";
                    levelPrefix = 1;
                    break;
                case GameMode.Deathmatch:
                    levelToLoad = "Deathmatch";
                    levelPrefix = 2;
                    break;
                case GameMode.PowerInsurgent:
                    levelToLoad = "Power Insurgent";
                    levelPrefix = 3;
                    break;
            }
        }

        FindObjectOfType<GameManager>().GetComponent<NetworkView>().RPC("NetworkLoadLevel", RPCMode.All, levelToLoad, levelPrefix);
    }

    [RPC]
    private void UpdateTimerText(string text)
    {
        countdownText.text = text;
    }

    public void BackButton()
    {
        string title = "LEAVE GAME";
        string message = "Do you really want to leave this game?";
        if (Network.isServer)
            message += " The server will be closed and all other players will automatically be kicked out of the server.";
        UnityAction yesAction = BackConfirm;
        UnityAction noAction = BackDecline;

        mainMenuHandler.ShowConfirmDialog(title, message, yesAction, noAction);
    }

    public void BackConfirm()
    {
        lobbyManager.leftGame = true;
        mainMenuHandler.GoToMainMenu();
        BackDecline();

        if (Network.isServer)
        {
            foreach (NetworkPlayer player in GetComponent<LobbyManager>().players)
            {
                if (player != Network.player)
                    Network.CloseConnection(player, true);
            }
        }
        Network.Disconnect(200);
    }

    public void BackDecline()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();
    }

    private void ClearPlayerList(List<Text> playerList)
    {
        foreach (Text playerText in playerList)
            playerText.text = "";
    }

    private void UpdatePlayerListTeam(List<Text> playerList, List<PlayerInfo> playerInfos)
    {
        foreach (PlayerInfo playerInfo in playerInfos)
        {
            if (playerInfo.position != 0)
            {
                if (playerInfo.position != 1 || playerInfo.position != 3)
                {
                    if (playerInfo.position == 2)
                    {
                        bool slotEmpty = true;
                        foreach (PlayerInfo playerInfo1 in playerInfos)
                        {
                            if (playerInfo == playerInfo1)
                                continue;
                            else if (playerInfo1.position == 1)
                            {
                                slotEmpty = false;
                                break;
                            }
                        }
                        if (slotEmpty)
                            playerInfo.position = 1;
                    }
                    else if (playerInfo.position == 4)
                    {
                        bool slotEmpty = true;
                        foreach (PlayerInfo playerInfo1 in playerInfos)
                        {
                            if (playerInfo == playerInfo1)
                                continue;
                            else if (playerInfo1.position == 3)
                            {
                                slotEmpty = false;
                                break;
                            }
                        }
                        if (slotEmpty)
                            playerInfo.position = 3;
                    }
                }
            }
        }

        ClearPlayerList(playerList);
        foreach (PlayerInfo playerInfo in playerInfos)
        {
            if (playerInfo.position != 0)
                playerList[playerInfo.position - 1].text = playerInfo.playerName;
        }

        UpdateReserveText();
    }

    private void UpdatePlayerList(List<Text> playerList, List<PlayerInfo> playerInfos)
    {
        if (Network.isServer)
            OrganizePlayerList(playerInfos);
        playerInfos.Sort((x, y) => x.position.CompareTo(y.position));

        ClearPlayerList(playerList);
        for (int currentPlayerIndex = 0; currentPlayerIndex < playerInfos.Count; currentPlayerIndex++)
            playerList[currentPlayerIndex].text = playerInfos[currentPlayerIndex].playerName;

        UpdateReserveText();
    }

    private void OrganizePlayerList(List<PlayerInfo> playerInfos)
    {
        playerInfos.Sort((x, y) => x.position.CompareTo(y.position));

        int newPosition = 1;
        foreach (PlayerInfo playerInfo in playerInfos)
        {
            playerInfo.position = newPosition;
            newPosition++;
        }
    }

    public void ChangeTeamSpeedster()
    {
        ChangeTeam(Team.Speedster, 1, 2);
    }

    public void ChangeTeamRoadkill()
    {
        ChangeTeam(Team.Roadkill, 3, 4);
    }

    private void ChangeTeam(Team team, int firstPosition, int secondPosition)
    {
        int playersInTeam = 0;
        PlayerInfo myPlayerInfo = null;
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
                myPlayerInfo = playerInfo;
            if (playerInfo.position == firstPosition || playerInfo.position == secondPosition)
                playersInTeam++;
        }

        if (playersInTeam == 0)
        {
            myPlayerInfo.position = firstPosition;
        }
        else if (playersInTeam < 2)
        {
            foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
            {
                if (playerInfo.position == firstPosition)
                {
                    myPlayerInfo.position = secondPosition;
                    break;
                }
                else if (playerInfo.position == secondPosition)
                {
                    myPlayerInfo.position = firstPosition;
                    break;
                }
            }
        }
        myPlayerInfo.GetComponent<CharacterTeam>().team = team;

        UpdatePlayerListTeam(playerListWithTeamText, lobbyManager.playerInfos);
        UpdateReserveText();
    }

    public void ChangeTeamReserve()
    {
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                playerInfo.position = 0;
                break;
            }
        }

        UpdateReserveText();
    }

    private int ReservesCount()
    {
        int reserves = 0;
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.position == 0)
                reserves++;
        }
        return reserves;
    }

    private void UpdateReserveText()
    {
        if (ReservesCount() > 1)
            reservePlayersText.text = ReservesCount() + " players in reserve.";
        else if (ReservesCount() == 1)
            reservePlayersText.text = ReservesCount() + " player in reserve.";
        else
            reservePlayersText.text = "";
    }

    public void ChangeGenderMale()
    {
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                if (playerInfo.gender != Gender.Male)
                    playerInfo.gender = Gender.Male;
                break;
            }
        }
    }

    public void ChangeGenderFemale()
    {
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                if (playerInfo.gender != Gender.Female)
                    playerInfo.gender = Gender.Female;
                break;
            }
        }
    }

    public void SelectKart()
    {
        PlayerInfo myPlayerInfo = null;
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                myPlayerInfo = playerInfo;
                break;
            }
        }

        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().teams)
            {
                case GameModeTeams.None:
                    if (FindKartMatches(myPlayerInfo) < 2)
                    {
                        myPlayerInfo.kart = myPlayerInfo.currentSelectedKart;
                        myPlayerInfo.kartVariation = FindKartMatches(myPlayerInfo);
                    }
                    break;
                case GameModeTeams.Two:
                    if (FindKartMatches(myPlayerInfo, myPlayerInfo.GetComponent<CharacterTeam>().team) < 1)
                    {
                        myPlayerInfo.kart = myPlayerInfo.currentSelectedKart;
                        myPlayerInfo.kartVariation = (int)(myPlayerInfo.GetComponent<CharacterTeam>().team);
                    }
                    break;
            }
        }
    }

    private int FindKartMatches(PlayerInfo myPlayerInfo)
    {
        int matches = 0;

        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo != myPlayerInfo)
            {
                if (playerInfo.kart == myPlayerInfo.currentSelectedKart)
                    matches++;
            }
        }

        return matches;
    }

    private int FindKartMatches(PlayerInfo myPlayerInfo, Team team)
    {
        int matches = 0;

        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo != myPlayerInfo && 
                playerInfo.GetComponent<CharacterTeam>().team == myPlayerInfo.GetComponent<CharacterTeam>().team)
            {
                if (playerInfo.kart == myPlayerInfo.currentSelectedKart)
                    matches++;
            }
        }

        return matches;
    }

    public void SelectKartLeft()
    {
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                if (playerInfo.currentSelectedKart > 0)
                    playerInfo.currentSelectedKart--;
                else
                    playerInfo.currentSelectedKart = (KartEnum)(System.Enum.GetValues(typeof(KartEnum)).Length - 1);
                break;
            }
        }
    }

    public void SelectKartRight()
    {
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                if ((int)(playerInfo.currentSelectedKart) < System.Enum.GetValues(typeof(KartEnum)).Length - 1)
                    playerInfo.currentSelectedKart++;
                else
                    playerInfo.currentSelectedKart = 0;
                break;
            }
        }
    }
}