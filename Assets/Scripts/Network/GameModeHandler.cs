using UnityEngine;

public enum GameMode
{
    TeamDeathmatch,
    Deathmatch,
    PowerInsurgent
}

public enum GameModeTeams
{
    None,
    Two
}

public class GameModeHandler : MonoBehaviour
{
    public GameMode gameMode;
    public GameModeTeams teams;

    void Start()
    {
        switch (gameMode)
        {
            case GameMode.Deathmatch:
                teams = GameModeTeams.None;
                break;
            case GameMode.PowerInsurgent:
                teams = GameModeTeams.Two;
                break;
            case GameMode.TeamDeathmatch:
                teams = GameModeTeams.Two;
                break;
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int net_gameMode = 0;
        int net_teams = 0;

        if (stream.isWriting)
        {
            net_gameMode = (int)gameMode;
            net_teams = (int)teams;
            stream.Serialize(ref net_gameMode);
            stream.Serialize(ref net_teams);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_gameMode);
            stream.Serialize(ref net_teams);
            gameMode = (GameMode)net_gameMode;
            teams = (GameModeTeams)net_teams;
        }
    }
}
