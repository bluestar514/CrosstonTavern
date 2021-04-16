using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WorldFactDisplayObj : MonoBehaviour
{
    public Text text;
    public WorldFact fact;

    public float fadeLevel = 1;

    public List<WorldFactGoal.Modifier> modifiers;

    public void Initiate(WorldFact fact)
    {
        this.fact = fact;
        this.text.text = fact.Verbalize("barkeep", "none"); 
    }

    public void MultiplyFadeLevel(float change)
    {
        fadeLevel *= change;

        UpdateAppearance();
    }
    public void ResetFadeLevel()
    {
        fadeLevel = 1;

        UpdateAppearance();
    }

    public void SetModifiers(List<WorldFactGoal.Modifier> mod)
    {
        modifiers = mod;

        UpdateAppearance();
    }

    public void RemoveModifier(WorldFactGoal.Modifier modifier)
    {
        modifiers.Remove(modifier);
        UpdateAppearance();
    }


    public void AddModifier(WorldFactGoal.Modifier modifier)
    {
        modifiers.Add(modifier);
        UpdateAppearance();
    }
    public void ResetModifiers()
    {
        modifiers = new List<WorldFactGoal.Modifier>();

        UpdateAppearance();
    }

    void UpdateAppearance()
    {

        Image img = GetComponent<Image>();

        if (modifiers.Contains(WorldFactGoal.Modifier.player)) {
            img.color = Color.yellow;
        }else if (modifiers.Contains(WorldFactGoal.Modifier.stuck)) {
            img.color = Color.red;
        } else {
            if(fadeLevel == 1)
                img.color = Color.white;
            else
                img.color = new Color(160f/255, 160f / 255, 180f / 255, 1);
        }

        text.color = new Color(50f / 255, 50f / 255, 50f / 255, 1);

        if (!modifiers.Contains(WorldFactGoal.Modifier.player)) {
            img.color *= new Color(1,1,1, fadeLevel);
            text.color *= fadeLevel;
        }

        
    }
}
