using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill 
{

    [SerializeField]
    StringFloatDictionary skillList;


    public Skill(StringFloatDictionary skillList)
    {
        this.skillList = skillList;
    }
    public Skill(): this(new StringFloatDictionary()){}

    public float GetSkillLevel(string skillId)
    {
        if (skillList.ContainsKey(skillId)) {
            return skillList[skillId];
        } else return 0;
    }

    public void Increase(string skillId, float count)
    {
        if (GetSkillLevel(skillId) != 0) skillList[skillId] += count;
        else skillList.Add(skillId, count);

        skillList[skillId] = Mathf.Max(0, skillList[skillId]);

        if (GetSkillLevel(skillId) == 0) skillList.Remove(skillId);
    }

    public Skill Copy(bool perfect)
    {
        if (perfect) {
            return new Skill(skillList);
        } else return new Skill();
    }
}
