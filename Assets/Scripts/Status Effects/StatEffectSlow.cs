using UnityEngine;

public class StatEffectSlow : StatEffect 
{
    internal float torqueDecrease;
    internal float topSpeedDecrease;

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

            kartController.motorTorque -= torqueDecrease;
            if (kartController.motorTorque <= 0)
                kartController.motorTorque = 0;

            kartController.topSpeed -= topSpeedDecrease;
            if (kartController.topSpeed <= 0)
                kartController.topSpeed = 0;
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
