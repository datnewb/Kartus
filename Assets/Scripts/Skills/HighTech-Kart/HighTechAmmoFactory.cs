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
        GetComponent<CharacterAmmo>().ammoRegenRate = originalAmmoRegenRate * ammoRegenRateMultiplier;
    }
}
