using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureContentPanel : MonoBehaviour
{
    public Text featureName;
    public Transform actionsHolder;
    public Transform itemsHolder;

    public GameObject actionObjPrefab;
    public GameObject itemObjPrefab;


    public void Init(Feature feature)
    {
        featureName.text = feature.id;

        foreach(GenericAction action in feature.providedActions) {
            AddAction(action.Id);
        }

    }

    void AddAction(string actionName)
    {
        Instantiate(actionObjPrefab, actionsHolder).GetComponent<SetText>().Set(actionName);
    }

    public void AddItem(string itemName)
    {
        Instantiate(itemObjPrefab, itemsHolder).GetComponent<SetText>().Set(itemName);
    }
}
