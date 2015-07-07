using UnityEngine;
using System.Collections;

public class StatEffectNaturesBlessing : StatEffect
{
    internal float torqueBoost;
    internal float topSpeedBoost;
    internal float healthRegenRate;

    private float originalTorque;
    private float originalTopSpeed;

    private bool blessingApplied;

    private KartController kartController;
    private CharacterHealth health;

    internal override void Start()
    {
        base.Start();

        duration = float.MaxValue;
        currentDuration = duration;

        kartController = GetComponent<KartController>();
        health = GetComponent<CharacterHealth>();

        originalTorque = kartController.motorTorque;
        originalTopSpeed = kartController.topSpeed;

        blessingApplied = false;

    }

    internal override void Effect()
    {
        base.Effect();

        currentDuration = duration;

        if (!blessingApplied)
        {
            kartController.motorTorque += torqueBoost;
            kartController.topSpeed += topSpeedBoost;

            blessingApplied = true;
        }

        health.currentHealth += healthRegenRate * Time.deltaTime;
        if (health.currentHealth >= health.maxHealth)
            health.currentHealth = health.maxHealth;
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();

        kartController.motorTorque = originalTorque;
        kartController.topSpeed = originalTopSpeed;
    }
}
