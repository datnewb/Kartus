using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyCharacterInfo : MonoBehaviour 
{
    [SerializeField]
    private Text kartNameText;
    [SerializeField]
    private Text kartDescText;

    [SerializeField]
    private List<SkillInfoUI> skillInfoUIs;

    void Update()
    {
        if (GetLocalPlayerInfo() != null)
        {
            if (FindObjectOfType<MenuLobby>() != null && FindObjectOfType<MenuLobby>().enabled)
            {
                foreach (Kart kart in FindObjectOfType<CharacterList>().karts)
                {
                    if (kart.kartEnumValue == GetLocalPlayerInfo().currentSelectedKart)
                    {
                        kartNameText.text = kart.kartName;
                        kartDescText.text = kart.kartDescription;

                        if (FindObjectOfType<MenuLobby>().kartPreview != null)
                        {
                            Skill[] skills = FindObjectOfType<MenuLobby>().kartPreview.GetComponents<Skill>();
                            for (int currentSkill = 0; currentSkill < skills.Length; currentSkill++)
                            {
                                skillInfoUIs[currentSkill].skillName.text = skills[currentSkill].skillName;
                                skillInfoUIs[currentSkill].skillImage.sprite = skills[currentSkill].skillIcon;
                                string skillType = "TYPE: " + skills[currentSkill].type;
                                if (skills[currentSkill].type == SkillType.Active)
                                {
                                    skillType += "\nCAST MODE: " + skills[currentSkill].castMode;
                                    skillType += "\nAMMO COST: " + skills[currentSkill].ammoCost;
                                    skillType += "\nCOOLDOWN: " + skills[currentSkill].coolDown + "s";
                                }
                                skillInfoUIs[currentSkill].skillType.text = skillType;
                                skillInfoUIs[currentSkill].skillDesc.text = skills[currentSkill].SkillDescription;
                            }
                        }
                    }
                }
            }
            else if (FindObjectOfType<GameManager>() != null)
            {
                if (FindObjectOfType<GameManager>().currentGameState == GameState.Game && FindObjectOfType<UIPauseMenu>().inPauseMenu)
                {
                    foreach (Kart kart in FindObjectOfType<CharacterList>().karts)
                    {
                        if (kart.kartEnumValue == GetLocalPlayerInfo().currentSelectedKart)
                        {
                            kartNameText.text = kart.kartName;
                            break;
                        }
                    }

                    if (GetLocalPlayerHandler() != null && GetLocalPlayerHandler().kart != null)
                    {
                        Skill[] skills = GetLocalPlayerHandler().kart.GetComponents<Skill>();
                        for (int currentSkill = 0; currentSkill < skills.Length; currentSkill++)
                        {
                            skillInfoUIs[currentSkill].skillName.text = skills[currentSkill].skillName;
                            skillInfoUIs[currentSkill].skillImage.sprite = skills[currentSkill].skillIcon;
                            string skillType = "TYPE: " + skills[currentSkill].type;
                            if (skills[currentSkill].type == SkillType.Active)
                            {
                                skillType += "\nCAST MODE: " + skills[currentSkill].castMode;
                                skillType += "\nAMMO COST: " + skills[currentSkill].ammoCost;
                                skillType += "\nCOOLDOWN: " + skills[currentSkill].coolDown + "s";
                            }
                            skillInfoUIs[currentSkill].skillType.text = skillType;
                            skillInfoUIs[currentSkill].skillDesc.text = skills[currentSkill].SkillDescription;
                        }
                    }
                    else
                    {
                        kartNameText.text = "";
                        foreach (SkillInfoUI skillInfoUI in skillInfoUIs)
                        {
                            skillInfoUI.skillDesc.text = "";
                            skillInfoUI.skillImage.sprite = null;
                            skillInfoUI.skillImage.color = new Color(0, 0, 0, 0);
                            skillInfoUI.skillName.text = "";
                            skillInfoUI.skillType.text = "";
                        }
                    }
                }
            }
        }
    }

    private PlayerInfo GetLocalPlayerInfo()
    {
        foreach (PlayerInfo playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            if (playerInfo.GetComponent<NetworkView>().isMine)
                return playerInfo;
        }
        return null;
    }

    private PlayerHandler GetLocalPlayerHandler()
    {
        foreach (PlayerHandler playerHandler in FindObjectsOfType<PlayerHandler>())
        {
            if (playerHandler.GetComponent<NetworkView>().isMine)
                return playerHandler;
        }
        return null;
    }
}

[System.Serializable]
public class SkillInfoUI
{
    [SerializeField]
    internal Text skillName;
    [SerializeField]
    internal Image skillImage;
    [SerializeField]
    internal Text skillType;
    [SerializeField]
    internal Text skillDesc;
}
