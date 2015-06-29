using UnityEngine;

public class Prototype_NetStarter : MonoBehaviour
{
    private int portNumber = 51234;
    private bool isInServer = false;
    private string ipAddress = "";

    void OnGUI()
    {
        if (!isInServer)
        {
            ipAddress = GUI.TextField(new Rect(200, 30, 200, 30), ipAddress);
            if (GUI.Button(new Rect(0, 0, 200, 30), "SERVER"))
                Network.InitializeServer(3, portNumber, false);
            else if (GUI.Button(new Rect(0, 30, 200, 30), "CLIENT"))
                Network.Connect(ipAddress, portNumber);
        }
        else
        {
            if (GUI.Button(new Rect(0, 0, 200, 30), "DISCONNECT"))
                Network.Disconnect(200);
        }
    }

    void OnServerInitialized()
    {
        isInServer = true;
    }

    void OnConnectedToServer()
    {
        isInServer = true;
    }

    void OnDisconnectedFromServer()
    {
        isInServer = false;
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.DestroyPlayerObjects(player);
        Network.RemoveRPCs(player);
    }
}
