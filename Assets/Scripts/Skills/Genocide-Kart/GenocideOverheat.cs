﻿using UnityEngine;
using System.Collections;

public class GenocideOverheat : Skill 
{
    public float shootCooldownLimit;
    public float shootCooldownIncreaseRate;
    public float shootCooldownDecreaseRate;

    private InputManager inputManager;
    private KartShoot kartShoot;
    private float originalShootCooldown;
    private float currentShootCooldown;

    internal override void Start()
    {
        base.Start();

        inputManager = GetComponent<InputManager>();
        kartShoot = GetComponent<KartShoot>();
        originalShootCooldown = kartShoot.shootCooldownTime;
    }

    internal override void PassiveEffect()
    {
        if (kartShoot != null)
        {
            currentShootCooldown = kartShoot.shootCooldownTime;
            if (inputManager.allowShoot)
            {
                if (Input.GetMouseButton(0))
                {
                    GunHeatup();
                }
                else
                {
                    GunCooldown();
                }
            }
            else
            {
                GunCooldown();
            }
            kartShoot.shootCooldownTime = currentShootCooldown;
            sliderValue = Mathf.InverseLerp(originalShootCooldown, shootCooldownLimit, currentShootCooldown);
        }
    }

    private void DisableShooting()
    {
        inputManager.allowShoot = false;
    }

    private void EnableShooting()
    {
        inputManager.allowShoot = true;
    }

    private void GunHeatup()
    {
        currentShootCooldown += shootCooldownIncreaseRate * Time.deltaTime;
        if (currentShootCooldown >= shootCooldownLimit)
        {
            DisableShooting();
            currentShootCooldown = shootCooldownLimit;
        }
    }

    private void GunCooldown()
    {
        currentShootCooldown -= shootCooldownDecreaseRate * Time.deltaTime;
        if (currentShootCooldown <= originalShootCooldown)
        {
            EnableShooting();
            currentShootCooldown = originalShootCooldown;
        }
    }

    internal override string SkillDescription
    {
        get
        {
            return "The longer the kart shoots, the hotter the kart's gun gets, which leads to an attack speed decrease of up to " + (shootCooldownLimit - originalShootCooldown)
                + " shots per second. When the kart reaches the limit, the kart will not be able to shoot until the kart's gun has fully cooled down.";
        }
        set { }
    }
}
