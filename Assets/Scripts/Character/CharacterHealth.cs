using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField]
    public GameObject destroyEffect;

    public float maxHealth;
    internal float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    [RPC]
    public void Damage(float value)
    {
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            if (destroyEffect != null)
                Network.Instantiate(destroyEffect, transform.position, transform.rotation, 0);
            Network.Destroy(gameObject);
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_currentHealth = 0;
        float net_maxHealth = 0;

        if (stream.isWriting)
        {
            net_currentHealth = currentHealth;
            net_maxHealth = maxHealth;

            stream.Serialize(ref net_currentHealth);
            stream.Serialize(ref net_maxHealth);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_currentHealth);
            stream.Serialize(ref net_maxHealth);

            currentHealth = net_currentHealth;
            maxHealth = net_maxHealth;
        }
    }
}
