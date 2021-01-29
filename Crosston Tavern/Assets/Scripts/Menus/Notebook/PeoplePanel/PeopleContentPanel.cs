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
    }

    public bool AddWorldFact(WorldFact fact)
    {

        if (fact is WorldFactGoal) {
            WorldFactGoal goalFact = (WorldFactGoal)fact;
            AddGoal(goalFact);
            return true;
        } else if (fact is WorldFactPreference) {
            WorldFactPreference preferenceFact = (WorldFactPreference)fact;

            AddPreference(preferenceFact.level, preferenceFact.item);
            return true;
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

    void AddPreference(PreferenceLevel level, string item) {
        if (level == PreferenceLevel.neutral) return;

        if (!preferences.ContainsKey(level)) {
            preferences.Add(level, new HashSet<string>());
        }

        preferences[level].Add(item);
        preferenceText[level].text = level + ": " + Verbalizer.MakeNiceList(new List<string>(preferences[level]));
    }
}
