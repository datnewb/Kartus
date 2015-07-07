using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour
{
    internal GameObject targetEnemy;

    internal KartGun kartGun;
    internal KartShoot kartShoot;
    internal NavMeshAgent navMeshAgent;

    IEnumerator minionCoroutine;

    void Start()
    {
        kartGun = GetComponent<KartGun>();
        kartShoot = GetComponent<KartShoot>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (Network.isServer)
        {
            minionCoroutine = MinionLogic();
            StartCoroutine(minionCoroutine);
        }
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

    internal void StopMinionLogic()
    {
        StopCoroutine(minionCoroutine);
    }

    private void Movement()
    {
        navMeshAgent.Resume();
        kartGun.AimAtPoint(transform.position + transform.forward * 1000);
    }

    private void Attack()
    {
        navMeshAgent.Stop();
        kartGun.AimAtPoint(targetEnemy.transform.position);
        if (kartShoot.canShoot)
            kartShoot.Shoot();
    }
}
