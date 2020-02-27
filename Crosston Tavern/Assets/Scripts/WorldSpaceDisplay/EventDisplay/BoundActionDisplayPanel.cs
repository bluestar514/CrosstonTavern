using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BoundActionDisplayPanel : SubDisplayPanel<BoundAction>
{
    public Text idText;
    public GameObject PreconditionsPanel;
    public GameObject PreconditionsPrefab;

    BoundAction action;

    public override void Set(BoundAction action)
    {
        this.action = action;

        Display();
    }

    void Display()
    {
        idText.text = action.ToString();

        foreach (Transform child in PreconditionsPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Condition condition in action.preconditions) {
            GameObject panel = Instantiate(PreconditionsPrefab, PreconditionsPanel.transform);
            panel.GetComponent<PreconditionsDisplayPanel>().Set(condition);
        }

    }

}
