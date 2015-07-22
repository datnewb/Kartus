using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField]
    public GameObject destroyEffect;

    public float maxHealth;
    internal float currentHealth;

    public bool isInvulnerable;
    internal bool isTargeted;

    private static float isTargetedClearTime = 5;
    private float currentIsTargetedClearTime = 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        ClearIsTargeted();
    }

    [RPC]
    public void Damage(float value)
    {
        if (!isInvulnerable)
        {
            currentHealth -= value;
            Targeted();
            if (currentHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, transform.rotation);
        if (GetComponent<PowerUpDropper>() != null)
            GetComponent<PowerUpDropper>().DropPowerUp();
        Network.Destroy(gameObject);
    }

    [RPC]
    public void Heal(float value)
    {
        currentHealth += value;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
    }

    private void Targeted()
    {
        isTargeted = true;
        currentIsTargetedClearTime = 0;
    }

    private void ClearIsTargeted()
    {
        if (isTargeted)
        {
            currentIsTargetedClearTime += Time.deltaTime;
            if (currentIsTargetedClearTime >= isTargetedClearTime)
            {
                currentIsTargetedClearTime = 0;
                isTargeted = false;
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_currentHealth = 0;
        float net_maxHealth = 0;
        bool net_isTargeted = false;

        if (stream.isWriting)
        {
            net_currentHealth = currentHealth;
            net_maxHealth = maxHealth;
            net_isTargeted = isTargeted;

            stream.Serialize(ref net_currentHealth);
            stream.Serialize(ref net_maxHealth);
            stream.Serialize(ref net_isTargeted);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_currentHealth);
            stream.Serialize(ref net_maxHealth);
            stream.Serialize(ref net_isTargeted);

            currentHealth = net_currentHealth;
            maxHealth = net_maxHealth;
            isTargeted = net_isTargeted;
        }
    }
}
