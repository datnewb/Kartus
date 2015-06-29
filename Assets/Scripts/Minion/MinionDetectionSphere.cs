using UnityEngine;

public class MinionDetectionSphere : MonoBehaviour
{
    private Minion minion;
    private int maskIgnoreBullet = ~((1 << 9) | (1 << 11));

    void Start()
    {
        minion = GetComponentInParent<Minion>();
    }

    void OnTriggerStay(Collider other)
    {
        if (Network.isServer)
        {
            if (minion.targetEnemy == null)
            {
                GameObject rootObject = other.transform.root.gameObject;
                if (rootObject.GetComponent<CharacterTeam>() != null)
                {
                    if (rootObject.GetComponent<CharacterTeam>().team != GetComponentInParent<CharacterTeam>().team)
                    {
                        minion.kartGun.DummyAimAtPoint(rootObject.transform.position);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(
                            minion.kartGun.dummyBulletSpawnPoint.position,
                            minion.kartGun.dummyBulletSpawnPoint.forward,
                            out hitInfo,
                            float.MaxValue,
                            maskIgnoreBullet))
                        {
                            if (hitInfo.transform.gameObject == rootObject)
                                minion.targetEnemy = rootObject;
                        }
                    }
                }
            }
            else
            {
                minion.kartGun.DummyAimAtPoint(minion.targetEnemy.transform.position);
                RaycastHit hitInfo;
                if (Physics.Raycast(
                    minion.kartGun.dummyBulletSpawnPoint.position,
                    minion.kartGun.dummyBulletSpawnPoint.forward,
                    out hitInfo,
                    float.MaxValue,
                    maskIgnoreBullet))
                {
                    if (hitInfo.transform.gameObject != minion.targetEnemy)
                        minion.targetEnemy = null;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (Network.isServer)
        {
            if (minion.targetEnemy != null)
            {
                if (other.gameObject == minion.targetEnemy)
                    minion.targetEnemy = null;
            }
        }
    }
}
