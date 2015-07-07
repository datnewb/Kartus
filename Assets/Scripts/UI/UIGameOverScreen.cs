using UnityEngine;

public class UIGameOverScreen : MonoBehaviour 
{
    [SerializeField]
    internal Canvas winScreen;
    [SerializeField]
    internal Canvas loseScreen;

    void Start()
    {
        winScreen.enabled = false;
        loseScreen.enabled = false;
    }

    internal void ShowGameOverScreen()
    {
        foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
            {
                bool isTeamGeneratorDestroyed = true;
                foreach (Generator generator in FindObjectsOfType<Generator>())
                {
                    if (generator.GetComponent<CharacterTeam>().team == playerInfo.GetComponent<CharacterTeam>().team)
                    {
                        isTeamGeneratorDestroyed = false;
                        break;
                    }
                }
                if (isTeamGeneratorDestroyed)
                {
                    winScreen.enabled = false;
                    loseScreen.enabled = true;
                }
                else
                {
                    winScreen.enabled = true;
                    loseScreen.enabled = false;
                }
                break;
            }
        }
    }
}
