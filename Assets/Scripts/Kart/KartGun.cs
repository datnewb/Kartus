using UnityEngine;

[RequireComponent(typeof(KartCamera))]
public class KartGun : MonoBehaviour 
{
    [SerializeField]
    private Transform gunYaw;
    [SerializeField]
    private Transform gunPitch;
    [SerializeField]
    internal Transform bulletSpawnPoint;

    [SerializeField]
    private Transform dummyGunYaw;
    [SerializeField]
    private Transform dummyGunPitch;
    [SerializeField]
    internal Transform dummyBulletSpawnPoint;

    public void DummyAimAtPoint(Vector3 point)
    {
        if (dummyGunYaw != null)
        {
            if (dummyGunYaw != dummyGunPitch)
            {
                dummyGunYaw.LookAt(point);
                dummyGunYaw.localEulerAngles = new Vector3(0, dummyGunYaw.localEulerAngles.y, 0);

                dummyGunPitch.LookAt(point);
                dummyGunPitch.localEulerAngles = new Vector3(dummyGunPitch.localEulerAngles.x, 0, 0);
            }
            else
            {
                dummyGunYaw.LookAt(point);
            }
            dummyBulletSpawnPoint.LookAt(point);
        }
    }

    public void AimAtPoint(Vector3 point)
    {
        if (gunYaw != gunPitch)
        {
            gunYaw.LookAt(point);
            gunYaw.localEulerAngles = new Vector3(0, gunYaw.localEulerAngles.y, 0);

            gunPitch.LookAt(point);
            gunPitch.localEulerAngles = new Vector3(gunPitch.localEulerAngles.x, 0, 0);
        }
        else
        {
            gunYaw.LookAt(point);
        }
        bulletSpawnPoint.LookAt(point);

        DummyAimAtPoint(point);
    }
}
