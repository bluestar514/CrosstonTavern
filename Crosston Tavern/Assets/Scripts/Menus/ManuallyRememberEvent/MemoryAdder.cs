using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MemoryAdder : MonoBehaviour
{
    WorldState globalState;
    Townie individual;

    public Dropdown addableFacts;
    public Transform contentPanel;
    public GameObject knownFactPrefab;

    public List<ExecutedAction> knownActions;
    public List<ExecutedAction> unknownActions;

    public void Setup(WorldState globalState, Townie individual)
    {
        addableFacts.ClearOptions();
        List<Transform> removeItems = new List<Transform>();
        foreach(Transform child in contentPanel) {
            if (child.GetComponent<KnownEvent>()) removeItems.Add(child);
        }
        foreach(Transform child in removeItems) {
            Destroy(child.gameObject);
        }


        this.globalState = globalState;
        this.individual = individual;

        knownActions = new List<ExecutedAction>();
        
        DetermineKnownAndUnknownActions();
    }

    public void DetermineKnownAndUnknownActions()
    {
        unknownActions = globalState.knownFacts.GetHistory();

        addableFacts.AddOptions(new List<string>(from executedAction in unknownActions
                                                 select executedAction.ToString()));

        foreach (ExecutedAction executedAction in individual.ws.knownFacts.GetHistory()) {
            DisplayAddedEvent(executedAction);
        }
    }

    void AddEvent(ExecutedAction executedAction)
    {
        WeightedAction action = executedAction.Action;

        List<Effect> realizedEffects = action.potentialOutcomes[0].effects;
        BoundBindingCollection bindings = action.Bindings;
        FeatureResources resources = globalState.map.GetFeature(action.FeatureId).relevantResources;

        foreach (Effect effect in realizedEffects) {
            effect.ExecuteEffect(individual.ws, individual, bindings, resources);
        }

        individual.ws.AddHistory(executedAction);

        DisplayAddedEvent(executedAction);
    }

    void DisplayAddedEvent(ExecutedAction executedAction)
    {
        knownActions.Add(executedAction);
        GameObject newEvent = Instantiate(knownFactPrefab, contentPanel);

        newEvent.GetComponent<KnownEvent>().Setup(executedAction);

        unknownActions.Remove(executedAction);

        Dropdown.OptionData optionData = new Dropdown.OptionData();
        foreach(Dropdown.OptionData data in addableFacts.options) {
            Debug.Log(executedAction.ToString() + " =?= "+ data.text);
            if (data.text == executedAction.ToString()) {
                optionData = data;
                break;
            }
        }

        addableFacts.options.Remove(optionData);
        addableFacts.RefreshShownValue();

    }

    public void ButtonAddEvent()
    {
        string chosen = addableFacts.options[addableFacts.value].text;

        ExecutedAction executedAction = new List<ExecutedAction>(from action in unknownActions
                                                                 where action.ToString() == chosen
                                                                 select action)[0];

        AddEvent(executedAction);
    }
}
