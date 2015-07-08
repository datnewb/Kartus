using UnityEngine;

public class TowerDetection : MonoBehaviour 
{
    private Tower tower;
    private int maskIgnoreBulletDetectionTower = ~((1 << 9) | (1 << 11) | (1 << 12));

    void Start()
    {
        tower = GetComponentInParent<Tower>();
    }

    void OnTriggerStay(Collider other)
    {
        if (Network.isServer)
        {
            if (tower.targetEnemy == null)
            {
                GameObject rootObject = other.transform.root.gameObject;
                if (rootObject.GetComponent<CharacterTeam>() != null)
                {
                    if (rootObject.GetComponent<CharacterTeam>().team != GetComponentInParent<CharacterTeam>().team &&
                        !rootObject.GetComponent<CharacterHealth>().isInvulnerable)
                    {
                        RaycastHit hitInfo;
                        if (Physics.Raycast(
                            tower.lightningSource.position,
                            (rootObject.transform.position - tower.lightningSource.position) / Vector3.Distance(rootObject.transform.position, tower.lightningSource.position),
                            out hitInfo,
                            float.MaxValue,
                            maskIgnoreBulletDetectionTower))
                        {
                            if (hitInfo.transform.gameObject == rootObject)
                                tower.targetEnemy = rootObject;
                        }
                    }
                }
            }
            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(
                    tower.lightningSource.position,
                    (tower.targetEnemy.transform.position - tower.lightningSource.position) / Vector3.Distance(tower.targetEnemy.transform.position, tower.lightningSource.position),
                    out hitInfo,
                    float.MaxValue,
                    maskIgnoreBulletDetectionTower))
                {
                    if (hitInfo.transform.gameObject != tower.targetEnemy)
                        tower.targetEnemy = null;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (Network.isServer)
        {
            if (tower.targetEnemy != null)
            {
                if (other.transform.root.gameObject == tower.targetEnemy)
                    tower.targetEnemy = null;
            }
        }
    }
}
