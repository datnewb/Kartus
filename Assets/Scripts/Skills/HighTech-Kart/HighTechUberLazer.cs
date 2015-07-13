using UnityEngine;

public class HighTechUberLazer : Skill 
{
    public GameObject lazerLineObject;
    public float damagePerSecond;

    internal override void ActiveEffect()
    {
        lazerLineObject.GetComponent<LineRenderer>().SetPosition(0, GetComponent<KartGun>().bulletSpawnPoint.position);

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
            
            lazerLineObject.GetComponent<LineRenderer>().SetPosition(1, hitInfo.point);
        }
        else
        {
            lazerLineObject.GetComponent<LineRenderer>().SetPosition(1, GetComponent<KartGun>().bulletSpawnPoint.position + GetComponent<KartGun>().bulletSpawnPoint.forward * 1000);
        }
    }
}
