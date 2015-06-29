using UnityEngine;

public class Prototype_NetSpawn : MonoBehaviour
{
    public GameObject playerHandlerObject;
    public GameObject kartToSpawn;

    void OnServerInitialized()
    {
        StartGame();
    }

    void OnConnectedToServer()
    {
        StartGame();
    }

    void StartGame()
    {
        GameObject playerHandlerInstance = Network.Instantiate(playerHandlerObject, Vector3.zero, Quaternion.identity, 0) as GameObject;
        PlayerHandler playerHandler = playerHandlerInstance.GetComponent<PlayerHandler>();
        playerHandler.kart = kartToSpawn;
    }
}
