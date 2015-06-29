using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour
{
    internal GameObject targetEnemy;

    internal KartGun kartGun;
    internal KartShoot kartShoot;
    internal NavMeshAgent navMeshAgent;

    void Start()
    {
        kartGun = GetComponent<KartGun>();
        kartShoot = GetComponent<KartShoot>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (Network.isServer)
            StartCoroutine(MinionLogic());
    }

    IEnumerator MinionLogic()
    {
        while (true)
        {
            Movement();
            yield return null;
            while (targetEnemy != null)
            {
                Attack();
                yield return null;
            }
        }
    }

    private void Movement()
    {
        navMeshAgent.Resume();
    }

    private void Attack()
    {
        navMeshAgent.Stop();
        kartGun.AimAtPoint(targetEnemy.transform.position);
        if (kartShoot.canShoot)
            kartShoot.Shoot();
    }
}
