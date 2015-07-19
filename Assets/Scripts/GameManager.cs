using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    MainMenu,
    Game
}

public class GameManager : MonoBehaviour
{
    internal GameState currentGameState;
    internal GameState previousGameState;

    internal bool gameStarted;

    void Start()
    {
        currentGameState = GameState.MainMenu;
        previousGameState = GameState.MainMenu;
        gameStarted = false;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (currentGameState != previousGameState)
        {
            if (currentGameState == GameState.Game)
            {
                if (!gameStarted)
                    StartCoroutine(GameStart());
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                gameStarted = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            previousGameState = currentGameState;
        }
    }

    IEnumerator GameStart()
    {
        if (Network.isServer)
        {
            foreach (MinionSpawner minionSpawner in FindObjectsOfType<MinionSpawner>())
                minionSpawner.enabled = false;
        }
        yield return null;

        while (true)
        {
            bool allLoadingFinished = true;
            foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
            {
                if (!playerInfo.loadingFinished)
                {
                    allLoadingFinished = false;
                    break;
                }
            }

            if (allLoadingFinished)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        if (Network.isServer)
        {
            foreach (MinionSpawner minionSpawner in FindObjectsOfType<MinionSpawner>())
                minionSpawner.enabled = true;
        }
        gameStarted = true;
    }

    [RPC]
    public void NetworkLoadLevel(string level, int levelPrefix)
    {
        Network.SetLevelPrefix(levelPrefix);

        LoadLevel(level, levelPrefix);
    }

    public void LoadLevel(string level, int levelPrefix)
    {
        StartCoroutine(AsyncLoadLevel(level, levelPrefix));
    }

    IEnumerator AsyncLoadLevel(string level, int levelPrefix)
    {
        AsyncOperation loadLevel = Application.LoadLevelAsync(levelPrefix);
        if (FindObjectOfType<MainMenuHandler>() != null)
        {
            MainMenuHandler.DisableAllCanvases();
            FindObjectOfType<MainMenuHandler>().loadingScreenCanvas.enabled = true;
            FindObjectOfType<MainMenuHandler>().lobbyCanvas.enabled = false;
            while (!loadLevel.isDone)
            {
                yield return loadLevel.isDone;
                if (FindObjectOfType<MenuLoadingScreen>() != null)
                    FindObjectOfType<MenuLoadingScreen>().SetProgressBar(loadLevel);
            }
        }

        foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                playerInfo.loadingFinished = true;
                break;
            }
        }

        while (true)
        {
            bool allFinished = true;
            foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
            {
                if (!playerInfo.loadingFinished)
                {
                    allFinished = false;
                    break;
                }
            }

            if (allFinished)
                break;

            yield return null;
        }

        if (levelPrefix > 0)
            currentGameState = GameState.Game;
        else
            currentGameState = GameState.MainMenu;
        if (FindObjectOfType<MainMenuHandler>() != null)
            FindObjectOfType<MenuLoadingScreen>().enabled = false;

        foreach (GameManager gameManager in FindObjectsOfType<GameManager>())
        {
            if (gameManager != this)
                Destroy(gameManager);
        }
    }
}
