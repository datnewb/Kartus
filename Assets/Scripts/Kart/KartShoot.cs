﻿using UnityEngine;

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
    private float shootCooldownTime;
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
        shotBullet.GetComponent<Rigidbody>().AddForce(shotBullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);

        canShoot = false;

        Invoke("AllowShoot", shootCooldownTime);
    }

    private void AllowShoot()
    {
        canShoot = true;
    }
}