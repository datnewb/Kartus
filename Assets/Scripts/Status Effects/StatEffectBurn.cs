using UnityEngine;

public class StatEffectBurn : StatEffect
{
    public float damagePerSecond;

    internal override void Effect()
    {
        if (GetComponent<CharacterShield>() != null)
            GetComponent<NetworkView>().RPC("DamageShield", RPCMode.All, damagePerSecond * Time.deltaTime);
        else
            GetComponent<NetworkView>().RPC("Damage", RPCMode.All, damagePerSecond * Time.deltaTime);
    }
}
