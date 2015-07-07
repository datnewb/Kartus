using UnityEngine;
using System.Collections;

public enum GameState
{
    MainMenu,
    Game
}

public class GameManager : MonoBehaviour
{
    internal GameState currentGameState;
    internal GameState previousGameState;

    void Start()
    {
        currentGameState = GameState.MainMenu;
        previousGameState = GameState.MainMenu;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Application.loadedLevel > 0)
            currentGameState = GameState.Game;
        else
            currentGameState = GameState.MainMenu;


        if (currentGameState != previousGameState)
        {
            DestroyOtherGameManagers();
            if (currentGameState == GameState.Game)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            previousGameState = currentGameState;
        }
    }

    private void DestroyOtherGameManagers()
    {
        foreach (GameManager gameManager in FindObjectsOfType<GameManager>())
        {
            if (gameManager != this)
                Destroy(gameManager.gameObject);
        }
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
        loadLevel.allowSceneActivation = false;
        FindObjectOfType<MainMenuHandler>().loadingScreenCanvas.enabled = true;
        FindObjectOfType<MainMenuHandler>().lobbyCanvas.enabled = false;
        while (loadLevel.progress < 0.9f)
        {
            yield return loadLevel.isDone;
            if (FindObjectOfType<MenuLoadingScreen>() != null)
                FindObjectOfType<MenuLoadingScreen>().SetProgressBar(loadLevel);
        }
        foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                playerInfo.loadingFinished = true;
                break;
            }
        }
        while (!FindObjectOfType<MenuLoadingScreen>().allFinished)
        {
            yield return null;
            bool allFinished = true;
            foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
            {
                if (!playerInfo.loadingFinished)
                    allFinished = false;
            }
            FindObjectOfType<MenuLoadingScreen>().allFinished = allFinished;
        }
        loadLevel.allowSceneActivation = true;
    }
}
