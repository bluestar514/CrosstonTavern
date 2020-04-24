using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect 
{
    [SerializeField]
    protected string id;

    public Effect()
    {
    }


    public virtual Effect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new Effect();
    }

    public virtual Effect SpecifyEffect()
    {
        return new Effect();
    }

    public virtual bool GoalComplete(WorldState ws, Person actor)
    {
        return false;
    }

    public static List<string> BindId(List<string> idList, Dictionary<string, List<string>> resources)
    {
        List<string> newId = new List<string>();
        foreach (string id in idList) {
            string[] split = id.Split(',');
            
            foreach (string item in split) {
                if (item.StartsWith("#") && item.EndsWith("#")
                    && resources.ContainsKey(item.Trim('#'))) {
                    newId.AddRange(resources[item.Trim('#')]);
                } else {
                    newId.Add(item);
                }
            }
        }

        return newId;
    }
    public static string BindId(string item, Dictionary<string, List<string>> resources)
    {
        if (item.StartsWith("#") && item.EndsWith("#")
            && resources.ContainsKey(item.Trim('#'))) {
            return String.Join(",", resources[item.Trim('#')]);
        } else {
            return item;
        }

    }

    public override string ToString()
    {
        return "<GenericMicroEffect>";
    }
}

