using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static int portNumber = 54321;

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
}
