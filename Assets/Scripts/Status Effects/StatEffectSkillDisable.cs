using UnityEngine;

public class StatEffectSkillDisable : StatEffect 
{
    internal override void Start()
    {
        base.Start();

        if (GetComponent<Skill>() != null)
        {
            foreach (Skill skill in GetComponents<Skill>())
            {
                if (skill.type == SkillType.Active)
                    skill.enabled = false;
            }
            GetComponent<InputManager>().allowSkillUsage = false;
        }
    }

    internal override void EndEffect()
    {
        if (GetComponent<Skill>() != null)
        {
            foreach (Skill skill in GetComponents<Skill>())
            {
                if (skill.type == SkillType.Active)
                    skill.enabled = true;
            }
            GetComponent<InputManager>().allowSkillUsage = true;
        }

        base.EndEffect();
    }
}
