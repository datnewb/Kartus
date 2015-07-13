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
    public string skillName;
    public Sprite skillIcon;
    public SkillType type;
    public SkillCastMode castMode;
    public SkillPosition position;
    public float ammoCost;
    public float toggleAmmoCostPerSecond;
    public float coolDown;
    public bool needsSlider;

    internal CharacterAmmo characterAmmo;
    internal float currentCoolDown;
    internal KeyCode hotKey;
    internal bool isInCoolDown;

    internal bool isAiming;
    private bool prevIsAiming;

    internal bool isToggled;
    private bool prevIsToggled;

    internal float sliderValue;

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
            hotKey = KeyCode.Space;

        currentCoolDown = 0;
        isInCoolDown = false;
        isAiming = false;
        prevIsAiming = false;
        isToggled = false;
        prevIsToggled = false;
    }

    void Update()
    {
        switch (type)
        {
            case SkillType.Active:
                if (!isInCoolDown)
                {
                    if (isToggled)
                    {
                        if (!prevIsToggled)
                        {
                            if (characterAmmo.CheckAmmo(ammoCost))
                                prevIsToggled = true;
                            else
                                isToggled = false;
                        }
                        else
                        {
                            if (characterAmmo.UseAmmo(toggleAmmoCostPerSecond * Time.deltaTime))
                                ActiveEffect();
                            else
                            {
                                isToggled = false;
                                SetCoolDown();
                            }
                        }

                        prevIsToggled = true;
                    }
                    else
                    {
                        if (prevIsToggled)
                            SetCoolDown();
                        prevIsToggled = false;
                    }

                    if (isAiming)
                    {
                        if (!prevIsAiming)
                        {
                            if (characterAmmo.CheckAmmo(ammoCost))
                                prevIsAiming = true;
                            else
                                isAiming = false;
                        }
                    }
                    else
                    {
                        prevIsAiming = false;
                    }
                }
                else
                {
                    isAiming = false;
                    prevIsAiming = false;
                    isToggled = false;
                    prevIsToggled = false;

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

    internal void ReadyActiveEffect()
    {
        if (characterAmmo.CheckAmmo(ammoCost))
        {
            if (castMode == SkillCastMode.Aim)
            {
                characterAmmo.UseAmmo(ammoCost);
                ActiveEffect();
                isAiming = false;
                prevIsAiming = false;
                SetCoolDown();
            }
            else if (castMode == SkillCastMode.Instant)
            {
                characterAmmo.UseAmmo(ammoCost);
                ActiveEffect();
                SetCoolDown();
            }
        }
        else
        {
            isAiming = false;
            prevIsAiming = false;
        }
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
