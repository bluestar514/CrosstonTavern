using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierItemOpinion : ChanceModifier
{
    public string item;
    public string person;
    public float minValue;
    public float maxValue;

    public ChanceModifierItemOpinion(string item, string person, float minValue, float maxValue)
    {
        this.item = item;
        this.person = person;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public override float Chance(WorldState ws)
    {
        float value = 0;

        Person p = ws.registry.GetPerson(person);

        if (p.preferences[PreferenceLevel.liked].Contains(item)) {
            value += .5f;
        }
        if (p.preferences[PreferenceLevel.disliked].Contains(item)) {
            value -= .5f;
        }
        if (p.preferences[PreferenceLevel.loved].Contains(item)) {
            value += 1;
        }
        if (p.preferences[PreferenceLevel.hated].Contains(item)) {
            value -= 1;
        }
        if (p.NeedItem(item)) {
            value += .5f;
        }

        if (minValue <= value && value <= maxValue) return 1;
        else return 0;
    }
}
