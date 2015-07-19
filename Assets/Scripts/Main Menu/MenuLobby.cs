using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MenuLobby : MonoBehaviour
{
    [SerializeField]
    private Canvas playerListCanvas;
    [SerializeField]
    private Canvas characterInfoCanvas;

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

    [SerializeField]
    internal GameObject selectKartButton;
    [SerializeField]
    internal GameObject kartHelpButton;
    [SerializeField]
    internal GameObject selectKartLeftButton;
    [SerializeField]
    internal GameObject selectKartRightButton;
    [SerializeField]
    internal GameObject maleButton;
    [SerializeField]
    internal GameObject femaleButton;
    [SerializeField]
    internal GameObject genderText;
    [SerializeField]
    internal GameObject cancelSelectKartButton;
    [SerializeField]
    internal Text selectedKartText;
    [SerializeField]
    internal GameObject backButton;

    [SerializeField]
    private Transform kartPreviewTransform;
    internal GameObject kartPreview;

    private MainMenuHandler mainMenuHandler;
    private Canvas lobbyCanvas;
    private LobbyManager lobbyManager;
    private int previousKartVariation;

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
        KartHelpClose();

        countdownText.text = "";
        reservePlayersText.text = "";
        ClearPlayerList(playerListNoTeamText);
        ClearPlayerList(playerListWithTeamText);

        SetGameMode();
        SetIPAddress();
        SetPlayerListView();

        EnableKartSelection();
        EnableNonKartSelectionButtons();
        EnableTeamSelection();

        if (Network.isServer)
            startGameButton.SetActive(true);
        else if (Network.isClient)
            startGameButton.SetActive(false);

        kartPreview = null;
        previousKartVariation = 0;
    }

    void OnDisable()
    {
        Destroy(kartPreview);
    }

    void Update()
    {
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().teams)
            {
                case GameModeTeams.None:
                    if (GetLocalPlayerInfo() != null && !GetLocalPlayerInfo().kartSelected)
                    {
                        if (FindKartMatches(GetLocalPlayerInfo()) > 0)
                        {
                            foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
                            {
                                if (playerInfo == GetLocalPlayerInfo())
                                    continue;
                                if (playerInfo.position > GetLocalPlayerInfo().position)
                                    GetLocalPlayerInfo().kartVariation = 0;
                                else
                                    GetLocalPlayerInfo().kartVariation = 1;
                            }
                        }
                        else
                            GetLocalPlayerInfo().kartVariation = 0;
                    }
                    UpdatePlayerList(playerListNoTeamText, lobbyManager.playerInfos);
                    break;
                case GameModeTeams.Two:
                    if (GetLocalPlayerInfo() != null)
                    {
                        if (!GetLocalPlayerInfo().kartSelected)
                        {
                            EnableTeamSelection();
                            if (GetLocalPlayerInfo().GetComponent<CharacterTeam>().team == Team.Speedster)
                                GetLocalPlayerInfo().kartVariation = 0;
                            else
                                GetLocalPlayerInfo().kartVariation = 1;
                        }
                    }

                    UpdatePlayerListTeam(playerListWithTeamText, lobbyManager.playerInfos);
                    break;
            }
        }

        SetPlayerListView();
        SetPlayerTeam();

        if (GetLocalPlayerInfo() != null)
        {
            if (GetLocalPlayerInfo().kartVariation != previousKartVariation)
            {
                Destroy(kartPreview);
                previousKartVariation = GetLocalPlayerInfo().kartVariation;
            }
        }
        ChangeKartPreview();
    }

    public void StartButton()
    {
        if (ReservesCount() > 0)
        {
            ErrorTeamNotSelected();
            return;
        }
        if (!CheckIfAllSelected())
        {
            ErrorKartNotSelected();
            return;
        }

        MainMenuHandler.DisableInputReceive();
        StartCoroutine(CountDownToStart());
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
        Network.Disconnect(200);
    }

    public void BackDecline()
    {
        Destroy(mainMenuHandler.dialogInstance);
        MainMenuHandler.EnableInputReceive();
    }

    public void ChangeTeamSpeedster()
    {
        ChangeTeam(Team.Speedster, 1, 2);
    }

    public void ChangeTeamRoadkill()
    {
        ChangeTeam(Team.Roadkill, 3, 4);
    }

    public void ChangeTeamReserve()
    {
        PlayerInfo myPlayerInfo = GetLocalPlayerInfo();
        myPlayerInfo.position = 0;

        UpdateReserveText();
    }

    public void ChangeGenderMale()
    {
        PlayerInfo myPlayerInfo = GetLocalPlayerInfo();
        if (myPlayerInfo.gender != Gender.Male)
        {
            myPlayerInfo.gender = Gender.Male;
            ChangeDriver();
        }
    }

    public void ChangeGenderFemale()
    {
        PlayerInfo myPlayerInfo = GetLocalPlayerInfo();
        if (myPlayerInfo.gender != Gender.Female)
        {
            myPlayerInfo.gender = Gender.Female;
            ChangeDriver();
        }
    }

    public void SelectKart()
    {
        PlayerInfo myPlayerInfo = GetLocalPlayerInfo();

        if (FindObjectOfType<GameModeHandler>() != null)
        {
            switch (FindObjectOfType<GameModeHandler>().teams)
            {
                case GameModeTeams.None:
                    if (FindKartMatches(myPlayerInfo) < 2)
                    {
                        myPlayerInfo.kart = myPlayerInfo.currentSelectedKart;
                        myPlayerInfo.kartVariation = FindKartMatches(myPlayerInfo);
                        goto default;
                    }
                    else
                        ErrorTooManyOfSameKart(FindObjectOfType<GameModeHandler>().teams);
                    break;
                case GameModeTeams.Two:
                    if (FindKartMatches(myPlayerInfo, myPlayerInfo.GetComponent<CharacterTeam>().team) < 1)
                    {
                        myPlayerInfo.kart = myPlayerInfo.currentSelectedKart;
                        myPlayerInfo.kartVariation = (int)(myPlayerInfo.GetComponent<CharacterTeam>().team);
                        goto default;
                    }
                    else
                        ErrorTooManyOfSameKart(FindObjectOfType<GameModeHandler>().teams);
                    break;
                default:
                    myPlayerInfo.kartSelected = true;
                    DisableTeamSelection();
                    DisableKartSelection();
                    characterInfoCanvas.enabled = false;
                    playerListCanvas.enabled = true;
                    break;
            }
        }
    }

    public void KartHelpOpen()
    {
        characterInfoCanvas.enabled = true;
        playerListCanvas.enabled = false;
    }

    public void KartHelpClose()
    {
        characterInfoCanvas.enabled = false;
        playerListCanvas.enabled = true;
    }

    public void CancelSelectKart()
    {
        GetLocalPlayerInfo().kartSelected = false;
        EnableTeamSelection();
        EnableKartSelection();
    }

    public void SelectKartLeft()
    {
        PlayerInfo myPlayerInfo = GetLocalPlayerInfo();
        if (myPlayerInfo.currentSelectedKart > 0)
            myPlayerInfo.currentSelectedKart--;
        else
            myPlayerInfo.currentSelectedKart = (KartEnum)(System.Enum.GetValues(typeof(KartEnum)).Length - 1);
        Destroy(kartPreview);
        ChangeKartPreview();
    }

    public void SelectKartRight()
    {
        PlayerInfo myPlayerInfo = GetLocalPlayerInfo();
        if ((int)(myPlayerInfo.currentSelectedKart) < System.Enum.GetValues(typeof(KartEnum)).Length - 1)
            myPlayerInfo.currentSelectedKart++;
        else
            myPlayerInfo.currentSelectedKart = 0;
        Destroy(kartPreview);
        ChangeKartPreview();
    }

    IEnumerator CountDownToStart()
    {
        GetComponent<NetworkView>().RPC("DisableTeamSelection", RPCMode.All);
        GetComponent<NetworkView>().RPC("DisableNonKartSelectionButtons", RPCMode.All);
        GetComponent<NetworkView>().RPC("DisableKartCancelButton", RPCMode.All);        
        for (int currentTime = 5; currentTime >= 0; currentTime--)
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
        if (GetLocalPlayerInfo() != null)
        {
            if (GetLocalPlayerInfo().position == 1 || GetLocalPlayerInfo().position == 2)
                GetLocalPlayerInfo().GetComponent<CharacterTeam>().team = Team.Speedster;
            else if (GetLocalPlayerInfo().position == 3 || GetLocalPlayerInfo().position == 4)
                GetLocalPlayerInfo().GetComponent<CharacterTeam>().team = Team.Roadkill;
            else
                GetLocalPlayerInfo().GetComponent<CharacterTeam>().team = Team.NeutralHostile;
        }
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
        else
            return;
        myPlayerInfo.GetComponent<CharacterTeam>().team = team;

        UpdatePlayerListTeam(playerListWithTeamText, lobbyManager.playerInfos);
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

    private int FindKartMatches(PlayerInfo myPlayerInfo)
    {
        int matches = 0;

        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (playerInfo != myPlayerInfo)
            {
                if (playerInfo.kart == myPlayerInfo.currentSelectedKart && playerInfo.kartSelected)
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
                playerInfo.GetComponent<CharacterTeam>().team == myPlayerInfo.GetComponent<CharacterTeam>().team
                 && playerInfo.kartSelected)
            {
                if (playerInfo.kart == myPlayerInfo.currentSelectedKart)
                    matches++;
            }
        }

        return matches;
    }

    private PlayerInfo GetLocalPlayerInfo()
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
        return myPlayerInfo;
    }

    private void EnableKartSelection()
    {
        selectKartButton.SetActive(true);
        kartHelpButton.SetActive(true);
        selectKartLeftButton.SetActive(true);
        selectKartRightButton.SetActive(true);
        cancelSelectKartButton.SetActive(false);

        maleButton.SetActive(true);
        femaleButton.SetActive(true);
        genderText.SetActive(true);
    }

    private void DisableKartSelection()
    {
        selectKartButton.SetActive(false);
        kartHelpButton.SetActive(false);
        selectKartLeftButton.SetActive(false);
        selectKartRightButton.SetActive(false);
        cancelSelectKartButton.SetActive(true);

        maleButton.SetActive(false);
        femaleButton.SetActive(false);
        genderText.SetActive(false);
    }

    private void EnableTeamSelection()
    {
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            if (FindObjectOfType<GameModeHandler>().teams == GameModeTeams.Two)
            {
                speedsterButton.enabled = true;
                roadkillButton.enabled = true;
                reserveButton.SetActive(true);
            }
            else
            {
                speedsterButton.enabled = false;
                roadkillButton.enabled = false;
                reserveButton.SetActive(false);
            }
        }
    }

    [RPC]
    private void DisableTeamSelection()
    {
        if (FindObjectOfType<GameModeHandler>() != null)
        {
            if (FindObjectOfType<GameModeHandler>().teams == GameModeTeams.Two)
            {
                speedsterButton.enabled = false;
                roadkillButton.enabled = false;
                reserveButton.SetActive(false);
            }
        }
    }

    private void EnableNonKartSelectionButtons()
    {
        backButton.SetActive(true);
        startGameButton.SetActive(true);
    }

    [RPC]
    private void DisableNonKartSelectionButtons()
    {
        backButton.SetActive(false);
        startGameButton.SetActive(false);
    }

    [RPC]
    private void DisableKartCancelButton()
    {
        cancelSelectKartButton.SetActive(false);
    }

    private bool CheckIfAllSelected()
    {
        foreach (PlayerInfo playerInfo in lobbyManager.playerInfos)
        {
            if (!playerInfo.kartSelected)
                return false;
        }
        return true;
    }

    private void ChangeKartPreview()
    {
        if (GetLocalPlayerInfo() != null)
        {
            if (kartPreview == null)
            {
                foreach (Kart kart in FindObjectOfType<CharacterList>().karts)
                {
                    if (kart.kartEnumValue == GetLocalPlayerInfo().currentSelectedKart)
                    {
                        kartPreview = Instantiate(kart.variations[GetLocalPlayerInfo().kartVariation], kartPreviewTransform.position, kartPreviewTransform.rotation) as GameObject;
                        selectedKartText.text = kart.kartName;
                        ChangeDriver();
                        RemoveComponentsFromPreview();
                        ObjectRotator rotator = kartPreview.AddComponent<ObjectRotator>();
                        rotator.Y = true;
                        rotator.anglePerSecond = -3;
                        if (FindObjectOfType<GameModeHandler>().teams == GameModeTeams.Two)
                            GetLocalPlayerInfo().kartVariation = (int)(GetLocalPlayerInfo().GetComponent<CharacterTeam>().team);
                        break;
                    }
                }
            }
        }
    }

    private void ChangeDriver()
    {
        Destroy(kartPreview.GetComponent<Driver>().driverInstance);
        if (FindObjectOfType<CharacterList>() != null)
        {
            kartPreview.GetComponent<Driver>().driverInstance = Instantiate(
                FindObjectOfType<CharacterList>().drivers[(int)GetLocalPlayerInfo().gender],
                kartPreview.transform.position,
                kartPreview.transform.rotation) as GameObject;
            kartPreview.GetComponent<Driver>().driverSeat = kartPreview.transform;
            kartPreview.GetComponent<Driver>().driverInstance.transform.parent = kartPreview.transform;
        }
    }

    private void RemoveComponentsFromPreview()
    {
        Destroy(kartPreview.GetComponent<KartCamera>().GetCameraRigRoot().gameObject);
        foreach (MonoBehaviour script in kartPreview.GetComponents<MonoBehaviour>())
        {
            if (script.GetType().BaseType != typeof(Skill) &&
                script.GetType() != typeof(KartGun) &&
                script.GetType() != typeof(Driver))
                Destroy(script);
        }
        Destroy(kartPreview.GetComponent<KartGun>());
        foreach (NetworkView networkView in kartPreview.GetComponents<NetworkView>())
            Destroy(networkView);
        foreach (NetworkView networkView in kartPreview.GetComponentsInChildren<NetworkView>())
            Destroy(networkView);
        foreach (WheelCollider wheelCollider in kartPreview.GetComponentsInChildren<WheelCollider>())
            Destroy(wheelCollider);
        Destroy(kartPreview.GetComponent<Rigidbody>());
        Destroy(kartPreview.GetComponentInChildren<NavMeshObstacle>());
    }

    private void ErrorKartNotSelected()
    {
        string title = "ERROR";
        string message = "Not all players have selected a kart. Wait for them to choose a kart.";
        UnityAction okAction = () =>
        {
            MainMenuHandler.EnableInputReceive();
            Destroy(mainMenuHandler.dialogInstance);
        };

        mainMenuHandler.ShowErrorDialog(title, message, okAction);
    }

    private void ErrorTeamNotSelected()
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

    private void ErrorTooManyOfSameKart(GameModeTeams teams)
    {
        string title = "ERROR";
        string message = "";
        switch (teams)
        {
            case GameModeTeams.None:
                message = "Two other players have already selected this kart. Choose something else.";
                break;
            case GameModeTeams.Two:
                message = "Your teammate has already selected this kart. Choose something else.";
                break;
        }
        UnityAction okAction = () =>
        {
            MainMenuHandler.EnableInputReceive();
            Destroy(mainMenuHandler.dialogInstance);
        };

        mainMenuHandler.ShowErrorDialog(title, message, okAction);
    }
}
