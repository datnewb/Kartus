using UnityEngine;

public class GameOver : MonoBehaviour 
{
    [RPC]
    private void GameFinish()
    {
        if (Network.isServer)
        {
            CleanUp();
            StopAllMinions();
            StopAllMinionSpawners();
            StopAllTowers();
        }
        DisableInput();
        RemoveKartScripts();
        FindObjectOfType<UIGameOverScreen>().ShowGameOverScreen();

        Invoke("BackToMainMenu", 5.0f);
    }

    private void RemoveKartScripts()
    {
        foreach (NetworkView networkView in FindObjectOfType<KartController>().GetComponents<NetworkView>())
            Destroy(networkView);
        foreach (MonoBehaviour script in FindObjectOfType<KartController>().GetComponents<MonoBehaviour>())
        {
            if (script.GetType() != typeof(KartGun))
                Destroy(script);
        }
    }

    private void CleanUp()
    {
        foreach (Bullet bullet in FindObjectsOfType<Bullet>())
            Network.Destroy(bullet.gameObject);
    }

    private void StopAllMinionSpawners()
    {
        foreach (MinionSpawner minionSpawner in FindObjectsOfType<MinionSpawner>())
            minionSpawner.StopSpawning();
    }

    private void StopAllMinions()
    {
        foreach (Minion minion in FindObjectsOfType<Minion>())
        {
            minion.StopMinionLogic();
            minion.GetComponent<NavMeshAgent>().Stop();
        }    
    }

    private void StopAllTowers()
    {
        foreach (Tower tower in FindObjectsOfType<Tower>())
            tower.StopTowerLogic();
    }

    private void DisableInput()
    {
        FindObjectOfType<InputManager>().allowInput = false;
    }

    private void BackToMainMenu()
    {
        Network.Disconnect();
        Application.LoadLevel(0);
    }
}
