using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour 
{
    [SerializeField]
    private float damage;
    internal float bulletLife = 1;

    [SerializeField]
    internal bool isExplosive;
    [SerializeField]
    internal float blastRadius;

    internal KartType ownerKartType;

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
                collision.gameObject.GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, damage);
            else if (collision.gameObject.GetComponent<CharacterHealth>() != null)
                collision.gameObject.GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damage);
            ApplyStatEffect(collision.gameObject);
            KillConfirm(collision.gameObject);
        }

        DestroyBullet();
    }

    private void KillConfirm(GameObject target)
    {
        if (target.GetComponent<CharacterHealth>().currentHealth <= 0)
        {
            switch (ownerKartType)
            {
                case KartType.Player:
                    if (target.GetComponent<KartInfo>() != null)
                    {
                        foreach (PlayerHandler playerHandler in FindObjectsOfType<PlayerHandler>())
                        {
                            if (playerHandler.GetComponent<NetworkView>().isMine)
                                playerHandler.kills++;
                            else if (playerHandler.GetComponent<NetworkView>().owner == target.GetComponent<NetworkView>().owner)
                                playerHandler.deaths++;
                        }
                    }
                    break;
                case KartType.Minion:
                    if (target.GetComponent<KartInfo>() != null)
                    {
                        foreach (PlayerHandler playerHandler in FindObjectsOfType<PlayerHandler>())
                        {
                            if (playerHandler.GetComponent<NetworkView>().owner == target.GetComponent<NetworkView>().owner)
                                playerHandler.deaths++;
                        }
                    }
                    break;
            }

        }
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
        if (isExplosive)
        {
            if (GetComponent<NetworkView>().isMine)
                Explode();
        }

        if (GetComponent<NetworkView>().isMine)
            Network.Destroy(gameObject);
    }

    private void Explode()
    {
        Collider[] collidersInBlastRadius = Physics.OverlapSphere(transform.position, blastRadius);
        List<GameObject> damagedObjects = new List<GameObject>();
        foreach (Collider collider in collidersInBlastRadius)
        {
            if (collider.transform.root.gameObject.GetComponent<CharacterTeam>() != null &&
                collider.transform.root.gameObject.GetComponent<CharacterTeam>().team != GetComponent<CharacterTeam>().team)
            {
                bool isDamaged = false;
                foreach (GameObject go in damagedObjects)
                {
                    if (collider.transform.root.gameObject == go)
                    {
                        isDamaged = true;
                        break;
                    }
                }
                if (!isDamaged)
                {
                    GameObject hitObject = collider.transform.root.gameObject;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(
                        transform.position,
                        (hitObject.transform.position - transform.position) / (hitObject.transform.position - transform.position).magnitude,
                        out hitInfo))
                    {
                        if (hitInfo.transform.root.gameObject == hitObject)
                        {
                            if (hitObject.GetComponent<CharacterShield>() != null)
                                hitObject.GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, damage);
                            else if (hitObject.GetComponent<CharacterHealth>() != null)
                                hitObject.GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damage);
                            ApplyStatEffect(hitObject);
                            KillConfirm(hitObject);
                            damagedObjects.Add(hitObject);
                        }
                    }
                }
            }
        }
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
                else if (statEffect.GetType() == typeof(StatEffectSlow))
                {
                    StatEffectSlow slow = null;
                    if (!statEffect.isStacking &&
                        target.GetComponent<StatEffectSlow>() != null)
                    {
                        slow = target.GetComponent<StatEffectSlow>();
                    }
                    else
                    {
                        slow = target.AddComponent<StatEffectSlow>();
                    }
                    if (slow != null)
                    {
                        slow.duration = statEffect.duration;
                        slow.torqueDecrease = ((StatEffectSlow)statEffect).torqueDecrease;
                        slow.topSpeedDecrease = ((StatEffectSlow)statEffect).topSpeedDecrease;
                        slow.statVisual = statEffect.statVisual;
                    }
                }
                else if (statEffect.GetType() == typeof(StatEffectSkillDisable))
                {
                    StatEffectSkillDisable skillDisable = null;
                    if (!statEffect.isStacking &&
                        target.GetComponent<StatEffectSkillDisable>() != null)
                    {
                        skillDisable = target.GetComponent<StatEffectSkillDisable>();
                    }
                    else
                    {
                        skillDisable = target.AddComponent<StatEffectSkillDisable>();
                    }
                    if (skillDisable != null)
                    {
                        skillDisable.duration = statEffect.duration;
                        skillDisable.statVisual = statEffect.statVisual;
                    }
                }
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_damage = 0;
        int net_ownerKartType = 0;

        if (stream.isWriting)
        {
            net_damage = damage;
            net_ownerKartType = (int)ownerKartType;
            stream.Serialize(ref net_damage);
            stream.Serialize(ref net_ownerKartType);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_damage);
            stream.Serialize(ref net_ownerKartType);
            ownerKartType = (KartType)net_ownerKartType;
            damage = net_damage;
        }
    }
}
