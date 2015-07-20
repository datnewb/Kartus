using UnityEngine;

public class HighTechEMPMissile : Skill 
{
    public GameObject empMissile;
    public Transform empMissileSpawnPoint;
    public float damage;
    public float empMissileSpeed;

    public float torqueDecrease;
    public float topSpeedDecrease;

    public float effectDuration;

    internal override void Start()
    {
        base.Start();
    }

    internal override void ActiveEffect()
    {
        GameObject empMissileInstance = Network.Instantiate(empMissile, GetComponent<KartGun>().bulletSpawnPoint.position, GetComponent<KartGun>().bulletSpawnPoint.rotation, 0) as GameObject;
        empMissileInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
        empMissileInstance.GetComponent<Bullet>().SetDamage(damage);
        empMissileInstance.GetComponent<Bullet>().ownerKartType = KartType.Player;
        empMissileInstance.GetComponent<Rigidbody>().AddForce(empMissileInstance.transform.forward * empMissileSpeed, ForceMode.VelocityChange);

        StatEffectSkillDisable skillDisable = empMissileInstance.AddComponent<StatEffectSkillDisable>();
        skillDisable.duration = effectDuration;

        StatEffectSlow slow = empMissileInstance.AddComponent<StatEffectSlow>();
        slow.duration = effectDuration;
    }

    internal override string SkillDescription
    {
        get
        {
            return "Shoots a missile which deals " + damage + " explosive damage. Enemy player karts hit gets a skill disable effect, " + torqueDecrease + " torque decrease, and "
                + topSpeedDecrease + " top speed decrease for " + effectDuration + " seconds.";
        }
        set { }
    }
}
