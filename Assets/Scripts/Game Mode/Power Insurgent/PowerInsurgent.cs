using UnityEngine;

public class PowerInsurgent : MonoBehaviour 
{
    bool gameFinished = false;

    void Update()
    {
        if (FindObjectsOfType<Generator>().Length < 2 && !gameFinished)
        {
            GetComponent<NetworkView>().RPC("GameFinish", RPCMode.All);
            gameFinished = true;
        }
    }
}
