using UnityEngine;
using System.Collections.Generic;

public class AdventureArrowsplosion : Skill 
{
    public List<Transform> arrowSpawnPoints;
    public GameObject arrow;
    public float damage;
    public float arrowSpeed;

    internal override void ActiveEffect()
    {
        foreach (Transform arrowSpawnPoint in arrowSpawnPoints)
        {
            GameObject arrowInstance = Network.Instantiate(arrow, arrowSpawnPoint.position, arrowSpawnPoint.rotation, 0) as GameObject;
            arrowInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
            arrowInstance.GetComponent<Bullet>().SetDamage(damage);
            arrowInstance.GetComponent<Bullet>().ownerKartType = KartType.Player;
            arrowInstance.GetComponent<Rigidbody>().AddForce(arrowInstance.transform.forward * arrowSpeed, ForceMode.VelocityChange);
        }
    }
}
