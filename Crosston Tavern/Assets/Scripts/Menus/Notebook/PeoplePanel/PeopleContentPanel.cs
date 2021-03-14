﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeopleContentPanel : MonoBehaviour
{
    public Text activePersonName;
    public Text lovedText;
    public Text likedText;
    public Text dislikedText;
    public Text hatedText;
    public Transform goalHolder;
    public AllRelationshipsPanel allRelationships;

    GameObject goalPrefab;

    Dictionary<PreferenceLevel, HashSet<string>> preferences;
    Dictionary<PreferenceLevel, Text> preferenceText;

    List<WorldFactDisplayObj> knownGoals = new List<WorldFactDisplayObj>();

    public void Init(string name, GameObject goalPrefab)
    {
        activePersonName.text = name;
        this.goalPrefab = goalPrefab;


        preferences = new Dictionary<PreferenceLevel, HashSet<string>>();
        preferenceText = new Dictionary<PreferenceLevel, Text>() {
            {PreferenceLevel.loved, lovedText },
            {PreferenceLevel.liked, likedText },
            {PreferenceLevel.disliked, dislikedText },
            {PreferenceLevel.hated, hatedText }
        };

        foreach(KeyValuePair<PreferenceLevel, Text> preferenceText in preferenceText) {
            Text text = preferenceText.Value;
            PreferenceLevel level = preferenceText.Key;

            text.text = level + ": Unknown";
        }


        allRelationships.Init(name);
    }

    public void AddMany(List<WorldFact> facts)
    {
        allRelationships.AddRelations(facts);
    }

    public bool AddWorldFact(WorldFact fact)
    {

        if (fact is WorldFactGoal) {
            WorldFactGoal goalFact = (WorldFactGoal)fact;

            if (FindFact(fact) == null) {
                AddGoal(goalFact);
                return true;
            } else {
                return false;
            }
        } else if (fact is WorldFactPreference) {
            WorldFactPreference preferenceFact = (WorldFactPreference)fact;

            return AddPreference(preferenceFact.level, preferenceFact.item);
            
        } else {
            return false;
        }

    }

    public bool RemoveWorldFact(WorldFact fact)
    {
        if (fact is WorldFactGoal) {
            WorldFactGoal goalFact = (WorldFactGoal)fact;
            RemoveGoal(goalFact);
            return true;
        } else if (fact is WorldFactPreference) {
            Debug.LogWarning("Trying to remove WorldFactPreference (" + fact + "). This is probably not desired and will result in unexpected behavior!");
            return false;
        } else {
            return false;
        }
    }

    void AddGoal(WorldFactGoal goal)
    {
        WorldFactDisplayObj panel = Instantiate(goalPrefab, goalHolder).GetComponent<WorldFactDisplayObj>();
        panel.Initiate(goal);
        knownGoals.Add(panel);
    }

    void RemoveGoal(WorldFactGoal goal)
    {
        WorldFactDisplayObj matchingFact = null;
        foreach (WorldFactDisplayObj panel in knownGoals) {
            if (panel.fact.Equals(goal)) {
                matchingFact = panel;
            }
        }

        if (matchingFact != null) {
            knownGoals.Remove(matchingFact);
            Destroy(matchingFact);
        }
    }

    bool AddPreference(PreferenceLevel level, string item) {
        if (level == PreferenceLevel.neutral) return false;

        if (!preferences.ContainsKey(level)) {
            preferences.Add(level, new HashSet<string>());
        }

        if (preferences[level].Contains(item)) return false;

        preferences[level].Add(item);
        preferenceText[level].text = level + ": " + Verbalizer.MakeNiceList(new List<string>(preferences[level]));

        return true;
    }


    WorldFactDisplayObj FindFact(WorldFact fact)
    {
        WorldFactDisplayObj matchingFact = null;
        if (fact is WorldFactGoal goalFact) {
            foreach (WorldFactDisplayObj panel in knownGoals) {
                if (panel.fact.Equals(goalFact)) {
                    matchingFact = panel;
                    break;
                }
            }
        }

        return matchingFact;
    }
}
