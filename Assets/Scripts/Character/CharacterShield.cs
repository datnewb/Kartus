using UnityEngine;

public class CharacterShield : MonoBehaviour
{
    public float maxShield;
    internal float currentShield;

    public float regenRate;
    public float regenDelay;
    internal float currentRegenDelay;
    internal bool isRegenerating;

    void Start()
    {
        currentShield = maxShield;
        currentRegenDelay = regenDelay;
        isRegenerating = false;
    }

    void Update()
    {
        if (isRegenerating)
        {
            currentShield += regenRate * Time.deltaTime;
            if (currentShield >= maxShield)
            {
                currentShield = maxShield;
                isRegenerating = false;
            }
        }
        else
        {
            if (currentShield != maxShield)
            {
                currentRegenDelay -= Time.deltaTime;
                if (currentRegenDelay <= 0)
                    isRegenerating = true;
            }
        }
    }

    [RPC]
    public void DamageShield(float value)
    {
        currentShield -= value;
        if (currentShield <= 0)
        {
            GetComponent<NetworkView>().RPC("Damage", RPCMode.All, -currentShield);
            currentShield = 0;
        }
        currentRegenDelay = regenDelay;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_currentShield = 0;
        float net_maxShield = 0;

        if (stream.isWriting)
        {
            net_currentShield = currentShield;
            net_maxShield = maxShield;

            stream.Serialize(ref net_currentShield);
            stream.Serialize(ref net_maxShield);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_currentShield);
            stream.Serialize(ref net_maxShield);

            currentShield = net_currentShield;
            maxShield = net_maxShield;
        }
    }
}
