using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetailPanel : DetailTab
{
    public GameObject InvalidActionsPanel;
    public GameObject RejectedActionsPanel;
    public GameObject RationalsPanel;
    public GameObject EffectsPanel;

    public GameObject InvalidActionsPrefab;
    public GameObject WeightedActionPrefab;
    public GameObject RationalsPrefab;
    public GameObject EffectsPrefab;

    public ExecutedAction action;


    public void Set(ExecutedAction action)
    {
        this.action = action;

        Display();
    }

    void Display()
    {
        displayName.text = action.ToString();

        ClearPanel(InvalidActionsPanel);
        ClearPanel(RejectedActionsPanel);
        ClearPanel(RationalsPanel);
        ClearPanel(EffectsPanel);


        FillPanel(InvalidActionsPanel, InvalidActionsPrefab, action.invalidChoices);
        FillPanel(RejectedActionsPanel, WeightedActionPrefab, action.rejectedChoices);
        FillPanel(RationalsPanel, RationalsPrefab, action.Action.weightRationals);
        FillPanel(EffectsPanel, EffectsPrefab, action.executedEffect.effects);

    }


    void ClearPanel(GameObject panel)
    {
        foreach (Transform child in panel.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void FillPanel<T>(GameObject parentPanel, GameObject prefab, List<T> contents)
    {
        foreach (T content in contents) {
            GameObject panel = Instantiate(prefab, parentPanel.transform);
            panel.GetComponent<SubDisplayPanel<T>>().Set(content);
        }
    }
}


