using UnityEngine;
using System.Collections;

public class NapalmIgnite : Skill
{
    public float damagePerSecond;
    public float duration;

    public GameObject burnVisual;

    internal override void PassiveEffect()
    {
        if (GetComponent<KartShoot>().shotBullet != null)
        {
            if (GetComponent<KartShoot>().shotBullet.GetComponent<StatEffectBurn>() == null)
            {
                StatEffectBurn burnEffect = GetComponent<KartShoot>().shotBullet.AddComponent<StatEffectBurn>();
                burnEffect.damagePerSecond = damagePerSecond;
                burnEffect.duration = duration;
                burnEffect.statVisual = burnVisual;
            }
        }
    }
}
