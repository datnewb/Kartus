using UnityEngine;
using System.Collections;

public class AdventureNaturesBlessing : Skill 
{
    public float torqueBoost;
    public float topSpeedBoost;
    public float healthRegenRate;

    internal override void PassiveEffect()
    {
        if (!GetComponent<CharacterHealth>().isTargeted)
        {
            if (GetComponent<StatEffectNaturesBlessing>() == null)
            {
                StatEffectNaturesBlessing naturesBlessing = gameObject.AddComponent<StatEffectNaturesBlessing>();
                naturesBlessing.torqueBoost = torqueBoost;
                naturesBlessing.topSpeedBoost = topSpeedBoost;
                naturesBlessing.healthRegenRate = healthRegenRate;
            }
        }
        else
        {
            if (GetComponent<StatEffectNaturesBlessing>() != null)
                Destroy(GetComponent<StatEffectNaturesBlessing>());
        }
    }
}
