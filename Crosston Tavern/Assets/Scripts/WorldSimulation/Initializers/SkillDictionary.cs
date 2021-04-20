using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary 
{

    static Dictionary<string, Dictionary<int, string>> skillLevels = new Dictionary<string, Dictionary<int, string>>() {
        {"fishing", new Dictionary<int, string>() {
            {0, "a beginner fisher" },
            {25, "a pretty good fisher" },
            {60, "a skilled fisher" },
            {90, "a master fisher" }
        } },
        {"farming", new Dictionary<int, string>() {
            {0, "a beginner farmer" },
            {25, "a pretty good farmer" },
            {60, "a skilled farmer" },
            {90, "a master farmer" }
        } },
        {"foraging", new Dictionary<int, string>() {
            {0, "a beginner forager" },
            {25, "a pretty good forager" },
            {60, "a skilled forager" },
            {90, "a master forager" }
        } },
        {"vitality", new Dictionary<int, string>() {
            {0, "unhealthy" },
            {2, "alright" },
            {5, "healthy" }
        } }
    };
    static public string Replace(string str, int value)
    {
        if (skillLevels.ContainsKey(str)) {
            List<int> thresholds = new List<int>(skillLevels[str].Keys);
            thresholds.Sort();
            thresholds.Reverse();

            foreach (int threshold in thresholds) {
                if (value > threshold) return skillLevels[str][threshold];
            }
        }

        return str;
    }
}
