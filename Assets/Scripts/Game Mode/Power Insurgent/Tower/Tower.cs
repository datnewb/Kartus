using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour 
{
    internal GameObject targetEnemy;

    [SerializeField]
    internal Transform lightningSource;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float attackCooldown;
    [SerializeField]
    private GameObject lightning;

    private IEnumerator towerCoroutine;

    void Start()
    {
        if (Network.isServer)
        {
            towerCoroutine = TowerLogic();
            StartCoroutine(towerCoroutine);
        }
    }

    IEnumerator TowerLogic()
    {
        while (true)
        {
            while (targetEnemy != null)
            {
                Attack();
                yield return new WaitForSeconds(attackCooldown);
            }
            yield return null;
        }
    }

    private void Attack()
    {
        GameObject lightningInstance = Network.Instantiate(lightning, Vector3.zero, Quaternion.identity, 0) as GameObject;
        CreateLightning(lightningInstance.GetComponent<LineRenderer>(), lightningSource.position, targetEnemy.transform.position);

        float finalDamage = 0;
        if (targetEnemy.GetComponent<KartInfo>() != null)
            finalDamage = damage * 1.5f;
        else
            finalDamage = damage;

        if (targetEnemy.GetComponent<CharacterShield>() != null)
            targetEnemy.GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, finalDamage);
        else if (targetEnemy.GetComponent<CharacterHealth>() != null)
            targetEnemy.GetComponent<NetworkView>().RPC("Damage", RPCMode.All, finalDamage);

        if (targetEnemy.GetComponent<CharacterHealth>().currentHealth <= 0)
        {
            if (targetEnemy.GetComponent<KartInfo>() != null)
            {
                foreach (PlayerHandler playerHandler in FindObjectsOfType<PlayerHandler>())
                {
                    if (playerHandler.GetComponent<NetworkView>().owner == targetEnemy.GetComponent<NetworkView>().owner)
                    {
                        playerHandler.deaths++;
                        break;
                    }
                }
            }
        }
    }

    private void CreateLightning(LineRenderer lineRenderer, Vector3 startPos, Vector3 endPos)
    {
        int linePoints = 16;
        for (int currentLinePoint = 0; currentLinePoint < linePoints; currentLinePoint++)
        {
            if (currentLinePoint == 0)
                lineRenderer.SetPosition(currentLinePoint, startPos);
            else if (currentLinePoint == 15)
                lineRenderer.SetPosition(currentLinePoint, endPos);
            else
            {
                Vector3 point = Vector3.Lerp(startPos, endPos, (float)currentLinePoint / (float)linePoints);
                float pointRandom = Vector3.Distance(startPos, endPos) / (float)linePoints / 3;
                point.y += Random.Range(-pointRandom, pointRandom);
                point.z += Random.Range(-pointRandom, pointRandom);
                lineRenderer.SetPosition(currentLinePoint, point);
            }
        }
    }

    internal void StopTowerLogic()
    {
        StopCoroutine(towerCoroutine);
    }
}
