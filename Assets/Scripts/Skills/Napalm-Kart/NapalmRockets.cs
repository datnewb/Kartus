using UnityEngine;
using System.Collections.Generic;

public class NapalmRockets : Skill 
{
    public List<Transform> rocketSpawnPoints;
    public GameObject rocket;
    public float damage;
    public float rocketSpeed;

    internal override void ActiveEffect()
    {
        foreach (Transform rocketSpawnPoint in rocketSpawnPoints)
        {
            GameObject rocketInstance = Network.Instantiate(rocket, rocketSpawnPoint.position, rocketSpawnPoint.rotation, 0) as GameObject;
            rocketInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
            rocketInstance.GetComponent<Bullet>().SetDamage(damage);
            rocketInstance.GetComponent<Bullet>().ownerKartType = KartType.Player;
            rocketInstance.GetComponent<Rigidbody>().AddForce(rocketInstance.transform.forward * rocketSpeed, ForceMode.VelocityChange);
        }
    }
}
