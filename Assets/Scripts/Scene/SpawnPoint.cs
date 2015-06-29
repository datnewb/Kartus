using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int position;
    private bool assigned;

    internal void Assign()
    {
        assigned = true;
    }

    internal void Unassign()
    {
        assigned = false;
    }

    internal bool IsAssigned()
    {
        return assigned;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        bool isAssigned = false;
        if (stream.isWriting)
        {
            isAssigned = assigned;
            stream.Serialize(ref isAssigned);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref isAssigned);
            assigned = isAssigned;
        }
    }
}
