using UnityEngine;

public enum SkillType
{
    Active,
    Passive
}

public enum SkillPosition
{
    First,
    Second,
    Passive
}

public enum SkillCastMode
{
    Instant,
    Aim,
    Toggle
}

public class Skill : MonoBehaviour
{
    public SkillType type;
    public SkillCastMode castMode;
    public SkillPosition position;
    public float ammoCost;
    public float toggleAmmoCostPerSecond;
    public float coolDown;

    internal CharacterAmmo characterAmmo;
    internal float currentCoolDown;
    internal KeyCode hotKey;
    internal bool isInCoolDown;
    internal bool isAiming;
    internal bool isToggled;
    internal bool toggleAmmoUsed;

    internal virtual void Start()
    {
        if (type == SkillType.Passive || position == SkillPosition.Passive)
        {
            type = SkillType.Passive;
            position = SkillPosition.Passive;
        }

        characterAmmo = GetComponent<CharacterAmmo>();

        if (position == SkillPosition.First)
            hotKey = KeyCode.LeftShift;
        else if (position == SkillPosition.Second)
            hotKey = KeyCode.LeftControl;

        currentCoolDown = 0;
        isInCoolDown = false;
        isAiming = false;
        isToggled = false;
        toggleAmmoUsed = false;
    }

    void Update()
    {
        switch (type)
        {
            case SkillType.Active:
                if (!isInCoolDown)
                {
                    if (Input.GetKeyDown(hotKey))
                    {
                        switch (castMode)
                        {
                            case SkillCastMode.Aim:
                                if (!isAiming)
                                {
                                    if (characterAmmo.CheckAmmo(ammoCost))
                                    {
                                        isAiming = true;
                                        return;
                                    }    
                                }
                                else
                                    isAiming = false;
                                break;
                            case SkillCastMode.Toggle:
                                if (!isToggled)
                                {
                                    if (characterAmmo.UseAmmo(ammoCost))
                                    {
                                        isToggled = true;
                                        return;
                                    }
                                }
                                break;
                            case SkillCastMode.Instant:
                                if (characterAmmo.UseAmmo(ammoCost))
                                {
                                    SetCoolDown();
                                    ActiveEffect();
                                }
                                break;
                        }
                    }

                    if (isToggled)
                    {
                        if (characterAmmo.UseAmmo(toggleAmmoCostPerSecond  * Time.deltaTime))
                            ActiveEffect();
                        else
                        {
                            isToggled = false;
                            SetCoolDown();
                        }

                        if (Input.GetKeyDown(hotKey))
                        {
                            isToggled = false;
                            SetCoolDown();
                        }
                    }
                    else if (isAiming)
                    {
                        if (characterAmmo.CheckAmmo(ammoCost))
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                characterAmmo.UseAmmo(ammoCost);
                                ActiveEffect();
                                isAiming = false;
                                SetCoolDown();
                            }
                        }
                        else
                        {
                            isAiming = false;
                        }

                        if (Input.GetKeyDown(hotKey))
                            isAiming = false;
                    }
                }
                else
                {
                    CoolDown();
                }
                break;

            case SkillType.Passive:
                PassiveEffect();
                break;
        }
    }

    internal virtual void PassiveEffect()
    {

    }

    internal virtual void ActiveEffect()
    {
        
    }

    internal void CoolDown()
    {
        currentCoolDown -= Time.deltaTime;
        if (currentCoolDown <= 0)
        {
            currentCoolDown = 0;
            isInCoolDown = false;
        }
    }

    internal void SetCoolDown()
    {
        currentCoolDown = coolDown;
        isInCoolDown = true;
    }
}
