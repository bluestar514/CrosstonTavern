using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetailPanel : DetailTab
{
    public GameObject RejectedActionsPanel;
    public GameObject RationalsPanel;
    public GameObject EffectsPanel;

    public GameObject WeightedActionPrefab;
    public GameObject RationalsPrefab;
    public GameObject EffectsPrefab;

    public ExecutedAction action;


    public void Set(ExecutedAction action)
    {
        this.action = action;
        print(action);

        Display();
    }

    void Display()
    {
        displayName.text = action.ToString();

        foreach (Transform child in RejectedActionsPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in RationalsPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in EffectsPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach (WeightedAction wa in action.rejectedChoices) {
            GameObject panel = Instantiate(WeightedActionPrefab, RejectedActionsPanel.transform);
            panel.GetComponent<WeightedActionPanel>().Set(wa);
        }
        foreach(WeightedAction.WeightRational rational in action.Action.weightRationals) {
            GameObject panel = Instantiate(RationalsPrefab, RationalsPanel.transform);
            panel.GetComponent<RationalPanel>().Set(rational);
        }
        foreach (MicroEffect effect in action.executedEffect.effects) {
            GameObject panel = Instantiate(EffectsPrefab, EffectsPanel.transform);
            panel.GetComponent<EffectPanel>().Set(effect);
        }
    }
}
