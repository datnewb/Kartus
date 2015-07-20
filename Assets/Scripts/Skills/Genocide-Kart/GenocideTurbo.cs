using UnityEngine;

public class GenocideTurbo : Skill 
{
    public float duration;

    public float torqueBoost;
    public float topSpeedBoost;

    internal override void Start()
    {
        base.Start();
    }

    internal override void ActiveEffect()
    {
        StatEffectSpeedBoost speedBoost = gameObject.AddComponent<StatEffectSpeedBoost>();
        speedBoost.torqueBoost = torqueBoost;
        speedBoost.topSpeedBoost = topSpeedBoost;
    }

    internal override string SkillDescription
    {
        get
        {
            return "Gives the kart " + torqueBoost + " torque boost and " + topSpeedBoost + " top speed boost.";
        }
        set { }
    }
}
