using UnityEngine;

public class StatEffectBurn : StatEffect
{
    public float damagePerSecond;

    internal override void Effect()
    {
        GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damagePerSecond * Time.deltaTime);
    }
}
