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
        featureName.text = VerbalizationDictionary.CapFirstLetter( VerbalizationDictionary.Replace(feature.id));

        foreach(GenericAction action in feature.providedActions) {
            BoundAction boundAction = new BoundAction(action, "...", feature.id, feature.location, new BoundBindingCollection(), action.verbilizationInfo);

            Verbalizer v = new Verbalizer("barkeeper", "none");

            AddAction(v.VerbalizeAction(boundAction, true));
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
