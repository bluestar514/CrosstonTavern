using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierItemOpinion : ChanceModifier
{
    public string item;
    public string person;
    public OpinionLevel minLevel;
    public OpinionLevel maxLevel;

    public enum OpinionLevel
    {
        liked = 1,
        disliked = -1,
        loved = 2,
        hated = -2,
        needed = 1,
        max=10,
        min=-10,
        neutral = 0
    }

    /// <summary>
    /// A deterministic chance modifier based on how much a person likes an item
    /// </summary>
    /// <param name="item">Id of the item in question</param>
    /// <param name="person">Person whose opinion we care about</param>
    /// <param name="minLevel">Lowest level of opinion we want to return 1 for</param>
    /// <param name="maxLevel">Highest level of opinion we want to return 1 for</param>
    public ChanceModifierItemOpinion(string item, string person, OpinionLevel minLevel, OpinionLevel maxLevel)
    {
        this.item = item;
        this.person = person;
        this.minLevel = minLevel;
        this.maxLevel = maxLevel;
    }

    public override string ToString()
    {
        return "ChanceModifierItemOpion(" + person + "," + item + ",{" + minLevel.ToString() + "~" + maxLevel.ToString() + "})>";
    }

    public override float Chance(WorldState ws)
    {
        int value = 0;

        Person p = ws.registry.GetPerson(person);

        switch (p.preference.GetLevel(item)) {
            case PreferenceLevel.loved:
                value += (int)OpinionLevel.loved;
                break;
            case PreferenceLevel.liked:
                value += (int)OpinionLevel.liked;
                break;
            case PreferenceLevel.disliked:
                value += (int)OpinionLevel.disliked;
                break;
            case PreferenceLevel.hated:
                value += (int)OpinionLevel.hated;
                break;
        }

        if (p.NeedItem(item)) {
            value += (int)OpinionLevel.needed;
        }

        if ((int)minLevel <= value && value <= (int)maxLevel) return 1;
        else return 0;
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        string item = string.Join(",", featureResources.BindString(bindings.BindString(this.item)));
        string person = bindings.BindString(this.person);

        return new ChanceModifierItemOpinion(item, person, minLevel, maxLevel);
    }

    public override List<Goal> MakeGoal(WorldState ws, float priority)
    {
        return base.MakeGoal(ws, priority);
    }
}
