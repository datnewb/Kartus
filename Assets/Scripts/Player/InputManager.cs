﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class InputManager : MonoBehaviour 
{
    [SerializeField]
    internal float mouseSensitivity;

    internal bool allowInput;

    internal bool allowShoot;
    internal bool allowDriving;
    internal bool allowAiming;

    internal Camera playerCamera;

    private int maskIgnoreKartBullet = ~((1 << 8) | (1 << 9) | (1 << 11));

    private KartController kartController;
    private KartShoot kartShoot;
    private KartCamera kartCamera;
    private KartGun kartGun;

    void Start()
    {
        allowInput = true;

        allowShoot = true;
        allowDriving = true;
        allowAiming = true;

        kartController = GetComponent<KartController>();
        kartShoot = GetComponent<KartShoot>();
        kartGun = GetComponent<KartGun>();
        kartCamera = GetComponent<KartCamera>();
            
        playerCamera = kartCamera.GetCamera();

    }

    void Update()
    {
        InputShoot();
        InputDrive();
        InputAim();
    }

    private void InputShoot()
    {
        if (allowShoot)
        {
            if (kartShoot.canShoot)
            {
                if (Input.GetMouseButton(0))
                    kartShoot.Shoot();
            }
        }
    }

    private void InputDrive()
    {
        if (allowDriving)
        {
            kartController.inputHorizontalValue = Input.GetAxis("Horizontal");

            kartController.inputVerticalValue = Input.GetAxis("Vertical");
            kartController.isInputVerticalPressed = Input.GetButton("Vertical");
        }
    }

    private void InputAim()
    {
        if (allowAiming)
        {
            kartCamera.mouseHorizontal = -Input.GetAxis("Mouse Y") * mouseSensitivity;
            kartCamera.mouseVertical = Input.GetAxis("Mouse X") * mouseSensitivity;

            Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            RaycastHit hitInfo;
            if (Physics.Raycast(cameraRay, out hitInfo, 100000f, maskIgnoreKartBullet))
                kartGun.AimAtPoint(hitInfo.point);
            else
                kartGun.AimAtPoint(playerCamera.transform.forward * 100000f + playerCamera.transform.position);
        }
    }
}