using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldHub : MonoBehaviour
{
    public Map map;
    public Registry registry;

    public List<WeightedAction> allWeightedActions = new List<WeightedAction>();
    public List<ChosenAction> chosenActions = new List<ChosenAction>();

    private void Start()
    {
        InitalizeWorld();

        foreach(Person person in registry.GetPeople()) {

            ActionHeuristicManager ahm = new ActionHeuristicManager(person, registry, map);

            List<WeightedAction> weightedActions = ahm.GenerateWeightedOptions();

            this.allWeightedActions.AddRange(weightedActions);

            chosenActions.Add(ahm.ChooseAction(weightedActions));
        }

        foreach(ChosenAction action in chosenActions) {


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
