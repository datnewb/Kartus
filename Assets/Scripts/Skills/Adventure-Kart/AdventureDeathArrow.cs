using UnityEngine;

public class AdventureDeathArrow : Skill 
{
    public GameObject deathArrow;
    public float damage;
    public float deathArrowSpeed;

    internal override void ActiveEffect()
    {
        GameObject deathArrowInstance = Network.Instantiate(deathArrow, GetComponent<KartGun>().bulletSpawnPoint.position, GetComponent<KartGun>().bulletSpawnPoint.rotation, 0) as GameObject;
        deathArrowInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
        deathArrowInstance.GetComponent<Bullet>().SetDamage(damage);
        deathArrowInstance.GetComponent<Bullet>().ownerKartType = KartType.Player;
        deathArrowInstance.GetComponent<Rigidbody>().AddForce(deathArrowInstance.transform.forward * deathArrowSpeed, ForceMode.VelocityChange);
    }
}
