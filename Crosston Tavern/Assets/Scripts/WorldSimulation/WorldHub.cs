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

    public WorldSpaceDisplayManager wsdm;

    private void Start()
    {
        InitalizeWorld();

        wsdm.AddPeople(new List<Person>(registry.GetPeople()));


        for (int i = 0; i < 1; i++) {
            TimeStep();
        }

    }

    public void TimeStep()
    {
        int i = timeStep.Count;

        List<ChosenAction> chosenActions = new List<ChosenAction>();
        foreach (Person person in registry.GetPeople()) {

            ActionHeuristicManager ahm = new ActionHeuristicManager(person, registry, map);

            List<WeightedAction> weightedActions = ahm.GenerateWeightedOptions();

            chosenActions.Add(ahm.ChooseAction(weightedActions));
        }

        List<ExecutedAction> executedActions = new List<ExecutedAction>();
        foreach (ChosenAction action in chosenActions) {
            WeightedAction weightedAction = action.Action;
            List<Effect> potentialEffects = weightedAction.potentialEffects;

            ActionExecutionManager aem = new ActionExecutionManager(registry.GetPerson(weightedAction.ActorId), registry, map);
            executedActions.Add(aem.ExecuteAction(action));
        }
        timeStep.Add(executedActions);

        foreach (ExecutedAction action in executedActions) {
            wsdm.AddEvent(action, i);
        }
    }

    void InitalizeWorld()
    {
        WorldInitializer wi = new WorldInitializer();
        List<Person> people = wi.InitializePeople();
        registry = wi.InitializeRegistry(people);
        map = wi.InitializeMap(people);
    }

}


