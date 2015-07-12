using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UICharacterSkills : MonoBehaviour 
{
    [SerializeField]
    private Image skillPassiveImage;

    [SerializeField]
    private List<ActiveSkillUI> skillActiveUI;

    private PlayerHandler playerHandler;

    void Start()
    {
        LookForPlayerHandler();
    }

    void Update()
    {
        if (playerHandler == null)
            LookForPlayerHandler();
        else
        {
            if (playerHandler.kartInstance != null)
            {
                foreach (Skill skill in playerHandler.kartInstance.GetComponents<Skill>())
                {
                    switch (skill.position)
                    {
                        case SkillPosition.Passive:
                            skillPassiveImage.sprite = skill.skillIcon;
                            break;
                        case SkillPosition.First:
                            skillActiveUI[0].skill = skill;
                            break;
                        case SkillPosition.Second:
                            skillActiveUI[1].skill = skill;
                            break;
                    }
                }
            }
            else
            {
                foreach (Skill skill in playerHandler.kart.GetComponents<Skill>())
                {
                    switch (skill.position)
                    {
                        case SkillPosition.Passive:
                            skillPassiveImage.sprite = skill.skillIcon;
                            break;
                        case SkillPosition.First:
                            skillActiveUI[0].skill = skill;
                            break;
                        case SkillPosition.Second:
                            skillActiveUI[1].skill = skill;
                            break;
                    }
                }
            }

            foreach (ActiveSkillUI activeSkill in skillActiveUI)
            {
                activeSkill.UpdateActiveSkillUI();
            }
        }
    }

    private void LookForPlayerHandler()
    {
        foreach (PlayerHandler playerHandlerInstance in FindObjectsOfType<PlayerHandler>())
        {
            if (playerHandlerInstance.GetComponent<NetworkView>().isMine)
            {
                playerHandler = playerHandlerInstance;
                break;
            }
        }
    }
}

[System.Serializable]
public class ActiveSkillUI
{
    internal Skill skill;

    [SerializeField]
    internal Image skillImage;
    [SerializeField]
    internal Image skillCooldownImage;
    [SerializeField]
    internal Text skillCooldownText;
    [SerializeField]
    internal Text ammoCostText;

    internal void UpdateActiveSkillUI()
    {
        if (skill != null)
        {
            if (skillImage.sprite == null)
            {
                skillImage.sprite = skill.skillIcon;
                ammoCostText.text = "" + skill.ammoCost;
            }
            if (skill.isInCoolDown)
            {
                skillCooldownText.text = "" + Mathf.RoundToInt(skill.currentCoolDown);
                skillCooldownImage.fillAmount = skill.currentCoolDown / skill.coolDown;
                skillImage.color = Color.gray;
            }
            else
            {
                skillCooldownText.text = "";
                skillCooldownImage.fillAmount = 0;
                skillImage.color = Color.white;
            }
        }
    }
}
