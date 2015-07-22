using UnityEngine;

public class PowerUpAmmo : PowerUp 
{
    public float ammo;

    internal override void Effect(GameObject target)
    {
        target.GetComponent<CharacterAmmo>().GainAmmo(ammo);

        base.Effect(target);
    }
}
