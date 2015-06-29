using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public GameObject playerHandlerPrefab;

    internal string playerName;
    internal Kart kart;
    internal Gender gender;
    internal int position;

    internal bool loadingFinished;
    private NetworkView myView;
    private PlayerHandler playerHandler;

    void Start()
    {
        myView = GetComponent<NetworkView>();
        position = FindObjectsOfType<PlayerInfo>().Length;
        loadingFinished = false;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (myView.isMine)
            myView.RPC("UpdateName", RPCMode.All, PlayerPrefs.GetString("playerName"));
        if (FindObjectOfType<GameManager>().currentGameState == GameState.Game)
        {
            if (playerHandler == null)
            {
                CreatePlayerHandler();
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int net_kart = 0;
        int net_gender = 0;
        int net_position = 0;
        bool net_loadingFinished = false;

        if (stream.isWriting)
        {
            net_kart = (int)kart;
            net_gender = (int)gender;
            net_position = position;
            net_loadingFinished = loadingFinished;

            stream.Serialize(ref net_kart);
            stream.Serialize(ref net_gender);
            stream.Serialize(ref net_position);
            stream.Serialize(ref net_loadingFinished);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_kart);
            stream.Serialize(ref net_gender);
            stream.Serialize(ref net_position);
            stream.Serialize(ref net_loadingFinished);

            kart = (Kart)net_kart;
            gender = (Gender)net_gender;
            position = net_position;
            loadingFinished = net_loadingFinished;
        }
    }

    private void CreatePlayerHandler()
    {
        playerHandler = gameObject.AddComponent<PlayerHandler>();
        playerHandler.playerInfo = this;

        playerHandler.kart = FindObjectOfType<CharacterList>().kartObjects[0];
        playerHandler.driver = FindObjectOfType<CharacterList>().drivers[0];
    }

    [RPC]
    internal void UpdateName(string newName)
    {
        playerName = newName;
    }
}
