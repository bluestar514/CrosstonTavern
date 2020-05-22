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

    public override float Chance(WorldState ws)
    {
        int value = 0;

        Person p = ws.registry.GetPerson(person);

        if (p.preferences[PreferenceLevel.liked].Contains(item)) {
            value += (int)OpinionLevel.liked;
        }
        if (p.preferences[PreferenceLevel.disliked].Contains(item)) {
            value -= (int)OpinionLevel.disliked;
        }
        if (p.preferences[PreferenceLevel.loved].Contains(item)) {
            value += (int)OpinionLevel.loved;
        }
        if (p.preferences[PreferenceLevel.hated].Contains(item)) {
            value -= (int)OpinionLevel.hated;
        }
        if (p.NeedItem(item)) {
            value += (int)OpinionLevel.needed;
        }

        if ((int)minLevel <= value && value <= (int)maxLevel) return 1;
        else return 0;
    }
}
