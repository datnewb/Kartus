using UnityEngine;

public class GenocideCorrosiveGas : Skill 
{
    public GameObject corrosiveBomb;
    public float damage;
    public float corrosiveBombSpeed;

    public float torqueDecrease;
    public float topSpeedDecrease;
    public float effectDuration;

    internal override void ActiveEffect()
    {
        GameObject corrosiveBombInstance = Network.Instantiate(corrosiveBomb, GetComponent<KartGun>().bulletSpawnPoint.position, GetComponent<KartGun>().bulletSpawnPoint.rotation, 0) as GameObject;
        corrosiveBombInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
        corrosiveBombInstance.GetComponent<Bullet>().SetDamage(damage);
        corrosiveBombInstance.GetComponent<Bullet>().ownerKartType = KartType.Player;
        corrosiveBombInstance.GetComponent<Rigidbody>().AddForce(corrosiveBombInstance.transform.forward * corrosiveBombSpeed, ForceMode.VelocityChange);

        StatEffectSlow slow = corrosiveBombInstance.AddComponent<StatEffectSlow>();
        slow.torqueDecrease = torqueDecrease;
        slow.topSpeedDecrease = topSpeedDecrease;
        slow.duration = effectDuration;
    }
}
