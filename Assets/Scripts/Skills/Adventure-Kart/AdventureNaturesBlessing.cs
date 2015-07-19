using UnityEngine;
using System.Collections;

public class AdventureNaturesBlessing : Skill 
{
    public float torqueBoost;
    public float topSpeedBoost;
    public float healthRegenRate;

    internal override void Start()
    {
        base.Start();

        skillDescription = "When out of combat, the Adventure kart gains the Nature's Blessing effect, which gives a torque increase of " 
            + torqueBoost + ", a top speed increase of " + topSpeedBoost + ", and " + healthRegenRate + "health per second regeneration.";
    }

    internal override void PassiveEffect()
    {
        if (GetComponent<CharacterHealth>() != null)
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
}
