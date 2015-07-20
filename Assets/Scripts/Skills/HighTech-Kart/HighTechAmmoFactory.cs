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
    }

    internal override void PassiveEffect()
    {
        if (GetComponent<CharacterAmmo>() != null)
            GetComponent<CharacterAmmo>().ammoRegenRate = originalAmmoRegenRate * ammoRegenRateMultiplier;
    }

    internal override string SkillDescription
    {
        get
        {
            return "Gives increased ammo regeneration. Instead of " + originalAmmoRegenRate + " ammo per second regeneration, the High-Tech kart has "
                + (originalAmmoRegenRate * ammoRegenRateMultiplier) + " ammo per second regeneration.";
        }
        set { }
    }
}
