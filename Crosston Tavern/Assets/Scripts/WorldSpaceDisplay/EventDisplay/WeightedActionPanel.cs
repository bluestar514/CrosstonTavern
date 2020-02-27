using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeightedActionPanel : SubDisplayPanel<WeightedAction>
{
    public Text idText;
    public GameObject RationalsPanel;
    public GameObject RationalsPrefab;

    WeightedAction action;

    public override void Set(WeightedAction action)
    {
        this.action = action;

        Display();
    }

    void Display()
    {
        idText.text = action.ToString();

        foreach (Transform child in RationalsPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (WeightedAction.WeightRational rational in action.weightRationals) {
            GameObject panel = Instantiate(RationalsPrefab, RationalsPanel.transform);
            panel.GetComponent<RationalPanel>().Set(rational);
        }

    }

}
