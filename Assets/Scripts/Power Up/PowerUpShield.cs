using UnityEngine;

public class PowerUpShield : PowerUp 
{
    public float shield;

    internal override void Effect(GameObject target)
    {
        target.GetComponent<NetworkView>().RPC("GainShield", RPCMode.All, shield);

        base.Effect(target);
    } 
}
