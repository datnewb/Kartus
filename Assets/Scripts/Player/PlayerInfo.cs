using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public GameObject playerHandlerPrefab;

    internal string playerName;
    internal KartEnum kart;
    internal Gender gender;
    internal int position;
    internal int kartVariation;
    internal bool kartSelected;

    internal bool loadingFinished;
    private NetworkView myView;
    private PlayerHandler playerHandler;

    internal KartEnum currentSelectedKart;

    void Start()
    {
        myView = GetComponent<NetworkView>();
        position = FindObjectsOfType<PlayerInfo>().Length;
        loadingFinished = false;
        kartSelected = false;
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
        int net_currentSelectedKart = 0;
        int net_gender = 0;
        int net_position = 0;
        bool net_kartSelected = false;
        bool net_loadingFinished = false;

        if (stream.isWriting)
        {
            net_kart = (int)kart;
            net_currentSelectedKart = (int)currentSelectedKart;
            net_gender = (int)gender;
            net_position = position;
            net_kartSelected = kartSelected;
            net_loadingFinished = loadingFinished;

            stream.Serialize(ref net_kart);
            stream.Serialize(ref net_currentSelectedKart);
            stream.Serialize(ref net_gender);
            stream.Serialize(ref net_position);
            stream.Serialize(ref net_kartSelected);
            stream.Serialize(ref net_loadingFinished);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_kart);
            stream.Serialize(ref net_currentSelectedKart);
            stream.Serialize(ref net_gender);
            stream.Serialize(ref net_position);
            stream.Serialize(ref net_kartSelected);
            stream.Serialize(ref net_loadingFinished);

            kart = (KartEnum)net_kart;
            currentSelectedKart = (KartEnum)net_currentSelectedKart;
            gender = (Gender)net_gender;
            position = net_position;
            kartSelected = net_kartSelected;
            loadingFinished = net_loadingFinished;
        }
    }

    private void CreatePlayerHandler()
    {
        playerHandler = gameObject.AddComponent<PlayerHandler>();
        playerHandler.playerInfo = this;

        foreach (Kart kartObject in FindObjectOfType<CharacterList>().karts)
        {
            if (kart == kartObject.kartEnumValue)
            {
                playerHandler.kart = kartObject.variations[kartVariation];
            }
        }
        playerHandler.driver = FindObjectOfType<CharacterList>().drivers[(int)gender];
    }

    [RPC]
    internal void UpdateName(string newName)
    {
        playerName = newName;
    }
}
