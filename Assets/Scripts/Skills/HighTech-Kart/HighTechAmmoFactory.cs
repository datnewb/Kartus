using UnityEngine;
using System.Collections;

public class HighTechAmmoFactory : Skill 
{
    public float ammoRegenRateMultiplier;
    internal float originalAmmoRegenRate;

    internal override void Start()
    {
        base.Start();
        originalAmmoRegenRate = GetComponent<CharacterAmmo>().ammoRegenRate;

        skillDescription = "Gives increased ammo regeneration. Instead of " + originalAmmoRegenRate + " ammo per second regeneration, the High-Tech kart has "
            + (originalAmmoRegenRate * ammoRegenRateMultiplier) + " ammo per second regeneration.";
    }

    internal override void PassiveEffect()
    {
        if (GetComponent<CharacterAmmo>() != null)
            GetComponent<CharacterAmmo>().ammoRegenRate = originalAmmoRegenRate * ammoRegenRateMultiplier;
    }
}
