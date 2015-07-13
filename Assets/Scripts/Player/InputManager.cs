using UnityEngine;
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
    internal bool allowSkillUsage;
    internal bool skillChanneling;

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
        allowSkillUsage = true;
        skillChanneling = false;

        kartController = GetComponent<KartController>();
        kartShoot = GetComponent<KartShoot>();
        kartGun = GetComponent<KartGun>();
        kartCamera = GetComponent<KartCamera>();
            
        playerCamera = kartCamera.GetCamera();

    }

    void Update()
    {
        if (allowInput)
        {
            InputShoot();
            InputDrive();
            InputAim();
            InputSkill();
        }
    }

    private void InputShoot()
    {
        if (allowShoot && !skillChanneling)
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

    private void InputSkill()
    {
        skillChanneling = false;
        foreach (Skill skill in GetComponents<Skill>())
        {
            if (skill.isChanneling && skill.castConfirmed)
            {
                skillChanneling = true;
                break;
            }
        }

        if (allowSkillUsage && !skillChanneling)
        {
            foreach (Skill skill in GetComponents<Skill>())
            {
                if (skill.type == SkillType.Passive)
                    continue;
                else
                {
                    if (Input.GetKeyDown(skill.hotKey))
                    {
                        switch (skill.castMode)
                        {
                            case SkillCastMode.Aim:
                                skill.isAiming = !skill.isAiming;
                                return;
                            case SkillCastMode.Instant:
                                skill.ReadyActiveEffect();
                                break;
                            case SkillCastMode.Toggle:
                                skill.isToggled = !skill.isToggled;
                                return;
                        }
                    }

                    if (skill.isAiming)
                    {
                        allowShoot = false;
                        if (Input.GetMouseButtonDown(0))
                            skill.ReadyActiveEffect();
                    }
                }
            }
        }
    }
}