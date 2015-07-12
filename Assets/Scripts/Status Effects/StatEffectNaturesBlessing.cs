using UnityEngine;
using System.Collections;

public class StatEffectNaturesBlessing : StatEffectSpeedBoost
{
    internal float healthRegenRate;

    internal override void Start()
    {
        base.Start();

        duration = float.MaxValue;
        currentDuration = duration;
    }

    internal override void Effect()
    {
        currentDuration = duration;

        GetComponent<NetworkView>().RPC("Heal", RPCMode.All, healthRegenRate * Time.deltaTime);
    }
}
