using UnityEngine;

public enum Team
{
    Speedster,
    Roadkill,
    NeutralHostile,
    NeutralFriendly
}

public class CharacterTeam : MonoBehaviour
{
    public Team team;

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int net_team = (int)(Team.NeutralFriendly);

        if (stream.isWriting)
        {
            net_team = (int)team;
            stream.Serialize(ref net_team);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_team);
            team = (Team)net_team;
        }
    }
}
