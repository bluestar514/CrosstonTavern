﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemPreference
{
    public PreferencesDictionary preferences;
    public Dictionary<string, PreferenceLevel> itemPreference;

    public ItemPreference()
    {
        this.preferences = new PreferencesDictionary() { };
        foreach (PreferenceLevel level in Enum.GetValues(typeof(PreferenceLevel))) {
            preferences.Add(level, new List<string>());
        }

        itemPreference = new Dictionary<string, PreferenceLevel>();

    }

    public ItemPreference Copy(bool perfect)
    {
        ItemPreference p = new ItemPreference();

        if (perfect) {
            p.preferences.CopyFrom(preferences);
            p.itemPreference = new Dictionary<string, PreferenceLevel>(itemPreference);
        }

        return p;
    }


    public void Add(string item, PreferenceLevel level)
    {
        if (itemPreference.ContainsKey(item)) {
            if (itemPreference[item] != level)
                throw new Exception("Item (" + item + ") already logged with a preference (" + itemPreference[item] + "). " +
                                    "Cannot add it again with preference " + level);
        } else {
            preferences[level].Add(item);
            itemPreference.Add(item, level);
        }
    }

    public PreferenceLevel GetLevel(string item)
    {
        if (itemPreference.ContainsKey(item))
            return itemPreference[item];
        else
            return PreferenceLevel.neutral;
    }
}
