using UnityEngine;
using System.Collections.Generic;

public class NapalmInferno : Skill 
{
    public float damage;
    public float blastRadius;
    public GameObject blastVisual;

    internal override void ActiveEffect()
    {
        Collider[] collidersInBlastRadius = Physics.OverlapSphere(transform.position, blastRadius);
        List<GameObject> damagedObjects = new List<GameObject>();
        foreach (Collider collider in collidersInBlastRadius)
        {
            if (collider.transform.root.gameObject.GetComponent<CharacterTeam>() != null &&
                collider.transform.root.gameObject.GetComponent<CharacterTeam>().team != GetComponent<CharacterTeam>().team)
            {
                bool isDamaged = false;
                foreach (GameObject go in damagedObjects)
                {
                    if (collider.transform.root.gameObject == go)
                    {
                        isDamaged = true;
                        break;
                    }
                }
                if (!isDamaged)
                {
                    GameObject hitObject = collider.transform.root.gameObject;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(
                        transform.position,
                        (hitObject.transform.position - transform.position) / (hitObject.transform.position - transform.position).magnitude,
                        out hitInfo))
                    {
                        if (hitInfo.transform.root.gameObject == hitObject)
                        {
                            if (hitObject.GetComponent<CharacterShield>() != null)
                                hitObject.GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, damage);
                            else if (hitObject.GetComponent<CharacterHealth>() != null)
                                hitObject.GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damage);
                            damagedObjects.Add(hitObject);
                        }
                    }
                }
            }
        }

        if (blastVisual != null)
            Network.Instantiate(blastVisual, transform.position, Quaternion.identity, 0);
    }
}
