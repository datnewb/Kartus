using UnityEngine;

public class HighTechUberLazer : Skill 
{
    public GameObject lazerLineObject;
    private GameObject lazerInstance;
    public float damagePerSecond;

    internal override void Update()
    {
        base.Update();

        if (castConfirmed)
        {
            if (lazerInstance == null)
                lazerInstance = Network.Instantiate(lazerLineObject, Vector3.zero, Quaternion.identity, 0) as GameObject;
        }
        else
        {
            if (lazerInstance != null)
            {
                Network.Destroy(lazerInstance);
                lazerInstance = null;
            }
        }
    }

    internal override void ActiveEffect()
    {
        if (lazerInstance != null)
            lazerInstance.GetComponent<LineRenderer>().SetPosition(0, GetComponent<KartGun>().bulletSpawnPoint.position);

        RaycastHit hitInfo;
        if (Physics.Raycast(GetComponent<KartGun>().bulletSpawnPoint.position, GetComponent<KartGun>().bulletSpawnPoint.forward, out hitInfo))
        {
            if (hitInfo.transform.root.gameObject.GetComponent<CharacterTeam>() != null)
            {
                if (hitInfo.transform.root.gameObject.GetComponent<CharacterTeam>().team != GetComponent<CharacterTeam>().team)
                {
                    GameObject hitObject = hitInfo.transform.root.gameObject;
                    if (hitObject.GetComponent<CharacterShield>() != null)
                        hitObject.GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, damagePerSecond * Time.deltaTime);
                    else if (hitObject.GetComponent<CharacterHealth>() != null)
                        hitObject.GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damagePerSecond * Time.deltaTime);
                }
            }

            if (lazerInstance != null)
                lazerInstance.GetComponent<LineRenderer>().SetPosition(1, hitInfo.point);
        }
        else
        {
            if (lazerInstance != null)
                lazerInstance.GetComponent<LineRenderer>().SetPosition(1, GetComponent<KartGun>().bulletSpawnPoint.position + GetComponent<KartGun>().bulletSpawnPoint.forward * 1000);
        }
    }
}
