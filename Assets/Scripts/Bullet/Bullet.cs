using UnityEngine;

public class Bullet : MonoBehaviour 
{
    [SerializeField]
    private float damage;
    internal float bulletLife = 1;

    void Start()
    {
        Invoke("DestroyBullet", bulletLife);
    }

    void Update()
    {
        LookForward();
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterTeam>() != null &&
            collision.gameObject.GetComponent<CharacterTeam>().team != GetComponent<CharacterTeam>().team)
        {
            if (collision.gameObject.GetComponent<CharacterShield>() != null)
            {
                collision.gameObject.GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, damage);
                ApplyStatEffect(collision.gameObject);
            }
            else if (collision.gameObject.GetComponent<CharacterHealth>() != null)
            {
                collision.gameObject.GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damage);
                ApplyStatEffect(collision.gameObject);
            }
        }

        DestroyBullet();
    }

    private void LookForward()
    {
        transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
    }

    internal void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void DestroyBullet()
    {
        if (GetComponent<NetworkView>().isMine)
            Network.Destroy(gameObject);
    }

    internal void ApplyStatEffect(GameObject target)
    {
        if (GetComponent<StatEffect>() != null)
        {
            foreach (StatEffect statEffect in GetComponents<StatEffect>())
            {
                if (statEffect.GetType() == typeof(StatEffectBurn))
                {
                    StatEffectBurn burnEffect = null;
                    if (!statEffect.isStacking &&
                        target.GetComponent<StatEffectBurn>() != null)
                    {
                        burnEffect = target.GetComponent<StatEffectBurn>();
                    }
                    else
                    {
                        burnEffect = target.AddComponent<StatEffectBurn>();
                    }
                    if (burnEffect != null)
                    {
                        burnEffect.duration = statEffect.duration;
                        burnEffect.damagePerSecond = ((StatEffectBurn)statEffect).damagePerSecond;
                        burnEffect.statVisual = statEffect.statVisual;
                    }
                }
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_damage = 0;

        if (stream.isWriting)
        {
            net_damage = damage;
            stream.Serialize(ref net_damage);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_damage);
            damage = net_damage;
        }
    }
}
