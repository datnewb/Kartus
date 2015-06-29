using UnityEngine;

public class CharacterAmmo : MonoBehaviour
{
    public float maxAmmo;
    internal float currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public bool UseAmmo(float ammoCost)
    {
        if (CheckAmmo(ammoCost))
        {
            currentAmmo -= ammoCost;
            return true;
        }
        else
            return false;
    }

    public bool CheckAmmo(float ammoCost)
    {
        if (currentAmmo - ammoCost >= 0)
            return true;
        else
            return false;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_currentAmmo = 0;
        float net_maxAmmo = 0;

        if (stream.isWriting)
        {
            net_currentAmmo = currentAmmo;
            net_maxAmmo = maxAmmo;

            stream.Serialize(ref net_currentAmmo);
            stream.Serialize(ref net_maxAmmo);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_currentAmmo);
            stream.Serialize(ref net_maxAmmo);

            currentAmmo = net_currentAmmo;
            maxAmmo = net_maxAmmo;
        }
    }
}
