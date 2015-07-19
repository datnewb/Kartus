using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPlayer : MonoBehaviour 
{
    [SerializeField]
    private Canvas waitingForOtherPlayersCanvas;
    [SerializeField]
    private Text loadedPlayersText;

    [SerializeField]
    private Canvas playerUI;

    void Start()
    {
        playerUI.enabled = false;
        waitingForOtherPlayersCanvas.enabled = true;
    }

    void Update()
    {
        if (FindObjectOfType<GameManager>() != null)
        {
            if (FindObjectOfType<GameManager>().gameStarted)
            {
                playerUI.enabled = true;
                waitingForOtherPlayersCanvas.enabled = false;
                enabled = false;
            }
            else
            {
                int loadedPlayers = 0;
                foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
                {
                    if (playerInfo.loadingFinished)
                        loadedPlayers++;
                }
                loadedPlayersText.text = loadedPlayers + "/" + FindObjectsOfType<PlayerInfo>().Length;
            }
        }
    }
}
