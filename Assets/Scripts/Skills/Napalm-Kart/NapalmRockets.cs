using UnityEngine;
using System.Collections;

public class NapalmRockets : Skill 
{
    public GameObject rocket;
    public Transform rocketSpawnPoint;
    public float damage;
    public float rocketSpeed;
    public int numberOfRockets;

    private float timeInterval;
    private IEnumerator rocketRoutine;

    internal override void Start()
    {
        base.Start();

        timeInterval = channelTime / ((float)numberOfRockets - 1.0f);
        rocketRoutine = null;
    }

    internal override void ActiveEffect()
    {
        if (rocketRoutine == null)
        {
            rocketRoutine = ShootRockets();
            StartCoroutine(rocketRoutine);
        }
    }

    IEnumerator ShootRockets()
    {
        for (int currentRocket = 0; currentRocket < numberOfRockets; currentRocket++)
        {
            GameObject rocketInstance = Network.Instantiate(rocket, rocketSpawnPoint.position, rocketSpawnPoint.rotation, 0) as GameObject;
            rocketInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
            rocketInstance.GetComponent<Bullet>().SetDamage(damage);
            rocketInstance.GetComponent<Bullet>().ownerKartType = KartType.Player;
            rocketInstance.GetComponent<Rigidbody>().AddForce(rocketInstance.transform.forward * rocketSpeed, ForceMode.VelocityChange);

            yield return new WaitForSeconds(timeInterval);
        }

        rocketRoutine = null;
    }
}
