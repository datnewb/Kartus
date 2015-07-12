using UnityEngine;

public class StatEffectSpeedBoost : StatEffect 
{
    internal float torqueBoost;
    internal float topSpeedBoost;

    internal float originalTorque;
    internal float originalTopSpeed;

    internal KartController kartController = null;

    internal override void Start()
    {
        base.Start();

        if (GetComponent<KartController>() != null)
        {
            kartController = GetComponent<KartController>();
            originalTorque = kartController.motorTorque;
            originalTopSpeed = kartController.topSpeed;

            kartController.motorTorque += torqueBoost;
            kartController.topSpeed += topSpeedBoost;
        }
    }

    internal override void EndEffect()
    {
        if (kartController != null)
        {
            kartController.motorTorque = originalTorque;
            kartController.topSpeed = originalTopSpeed;
        }

        base.EndEffect();
    }
}
