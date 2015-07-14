using UnityEngine;

enum KartType
{
    Player,
    Minion
}

[RequireComponent(typeof(KartGun))]
public class KartShoot : MonoBehaviour 
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private float bulletDamageMin;
    [SerializeField]
    private float bulletDamageMax;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    internal float shootCooldownTime;
    internal bool canShoot;

    internal GameObject shotBullet;

    private Transform bulletSpawnPoint;

    void Start()
    {
        canShoot = true;
        bulletSpawnPoint = GetComponent<KartGun>().bulletSpawnPoint;
    }

    internal void Shoot()
    {
        shotBullet = Network.Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation, 0) as GameObject;
        shotBullet.GetComponent<CharacterTeam>().team = GetComponent<CharacterTeam>().team;
        shotBullet.GetComponent<Bullet>().SetDamage(Random.Range(bulletDamageMin, bulletDamageMax));
        if (GetComponent<Minion>() != null)
            shotBullet.GetComponent<Bullet>().ownerKartType = KartType.Minion;
        else
            shotBullet.GetComponent<Bullet>().ownerKartType = KartType.Player;
        shotBullet.GetComponent<Rigidbody>().AddForce(shotBullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);

        canShoot = false;

        ShootCooldown(shootCooldownTime);
    }

    internal void ShootCooldown(float time)
    {
        Invoke("AllowShoot", time);
    }

    private void AllowShoot()
    {
        canShoot = true;
    }
}
