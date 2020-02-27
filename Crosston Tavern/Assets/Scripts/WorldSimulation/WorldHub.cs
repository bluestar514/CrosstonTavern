using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldHub : MonoBehaviour
{
    public Map map;
    public Registry registry;

    public List<List<ExecutedAction>> timeStep = new List<List<ExecutedAction>>();

    public LoadWorldData wi;
    public WorldSpaceDisplayManager wsdm;
    public TextAsset locationData;
    public TextAsset featureData;

    Dictionary<Person, ChosenAction> chosenActions = new Dictionary<Person, ChosenAction>();
    private void Start()
    {
        InitalizeWorld();

        wsdm.AddPeople(new List<Person>(registry.GetPeople()));

        
        foreach (Person person in registry.GetPeople()) {
            chosenActions.Add(person, null);
        }
        for (int i = 0; i < 1; i++) {
            TimeStep();
        }

    }

    public void TimeStep()
    {
        int i = timeStep.Count;

        List<ExecutedAction> executedActions = new List<ExecutedAction>();
        foreach (Person person in registry.GetPeople()) {

            if(chosenActions[person] == null) {
                ActionHeuristicManager ahm = new ActionHeuristicManager(person, registry, map);

                List<WeightedAction> weightedActions = ahm.GenerateWeightedOptions();

                chosenActions[person]= ahm.ChooseAction(weightedActions);
            }
            
            ChosenAction action = chosenActions[person];

            WeightedAction weightedAction = action.Action;
            List<Effect> potentialEffects = weightedAction.potentialEffects;

            ActionExecutionManager aem = new ActionExecutionManager(registry.GetPerson(weightedAction.ActorId), registry, map);

            ExecutedAction executedAction = aem.ExecuteAction(action);
            if(executedAction != null) {
                executedActions.Add(executedAction);

                chosenActions[person] = null;
            }
        }
        timeStep.Add(executedActions);

        foreach (ExecutedAction action in executedActions) {
            wsdm.AddEvent(action, i);
        }





    }

    void InitalizeWorld()
    {
        wi = new LoadWorldData(locationData, featureData); //new WorldInitializer();
        List <Person> people = wi.InitializePeople();
        registry = wi.InitializeRegistry(people);
        map = wi.InitializeMap(people);
    }

}

