using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    void Start()
    {
        Network.InitializeServer(0, NetworkManager.portNumber, false);
    }
}
