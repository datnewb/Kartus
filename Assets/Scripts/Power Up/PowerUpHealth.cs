using UnityEngine;

public class PowerUpHealth : PowerUp 
{
    public float health;

    internal override void Effect(GameObject target)
    {
        target.GetComponent<NetworkView>().RPC("Heal", RPCMode.All, health);

        base.Effect(target);
    }
}
