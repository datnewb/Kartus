using UnityEngine;
using System.Collections;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField]
    internal GameObject minion;

    [SerializeField]
    private int minionsPerSpawnGroup;

    [SerializeField]
    private float delayStartSpawn;
    [SerializeField]
    private float delaySpawnGroup;
    [SerializeField]
    private float delaySpawnUnit;

    [SerializeField]
    private Transform targetTransform;

    IEnumerator spawnerCoroutine;

    internal bool canSpawn;

    void Start()
    {
        canSpawn = true;
    }

    void OnEnable()
    {
        if (Network.isServer)
        {
            if (minion != null && targetTransform != null)
            {
                if (spawnerCoroutine == null)
                    spawnerCoroutine = Spawner();
                StartCoroutine(spawnerCoroutine);
            }
        }
    }

    void OnDisable()
    {
        StopCoroutine(spawnerCoroutine);
    }

    IEnumerator Spawner()
    {
        yield return new WaitForSeconds(delayStartSpawn);
        while (canSpawn)
        {
            for (int minion = 0; minion < minionsPerSpawnGroup; minion++)
            {
                Spawn();
                if (minion + 1 < minionsPerSpawnGroup)
                    yield return new WaitForSeconds(delaySpawnUnit);
            }
            yield return new WaitForSeconds(delaySpawnGroup);
        }
    }

    internal void StopSpawning()
    {
        StopCoroutine(spawnerCoroutine);
    }


    internal void Spawn()
    {
        GameObject minionInstance = Network.Instantiate(minion, transform.position, transform.rotation, 0) as GameObject;
        minionInstance.GetComponent<NavMeshAgent>().SetDestination(targetTransform.position);
        minionInstance.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
    }
}
