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
        if (FindObjectOfType<MenuLobby>().enabled && GetLocalPlayerInfo() != null)
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
                            skillInfoUIs[currentSkill].skillDesc.text = skills[currentSkill].skillDescription;
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
