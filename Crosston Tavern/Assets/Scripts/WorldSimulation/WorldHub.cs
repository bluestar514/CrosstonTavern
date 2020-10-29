﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldHub : MonoBehaviour
{
    public WorldState ws;

    List<Townie> allPeople;

    public List<List<ExecutedAction>> timeStep = new List<List<ExecutedAction>>();

    public WorldSpaceDisplayManager wsdm;

    Dictionary<Townie, ChosenAction> chosenActions = new Dictionary<Townie, ChosenAction>();
    private void Start()
    {
        ws = WorldStateInitializer.GetWorldState();
        ws.id = "main";

        allPeople = WorldStateInitializer.GetTownies(ws, transform); 
        wsdm.AddPeople(new List<Person>(ws.registry.GetPeople()));

        foreach (Townie person in allPeople) {
            chosenActions.Add(person, null);

            person.townieInformation.knownGoals = person.gm.GetGoalsList();

        }

        for (int i = 0; i < 1; i++) {
            TimeStep();
        }

    }

    public void TimeStep()
    {
        int i = timeStep.Count;

        
        List<ExecutedAction> executedActions = new List<ExecutedAction>();
        foreach (Townie person in allPeople) {
            

            if(chosenActions[person] == null) {
                person.townieInformation.knownGoals = person.gm.GetGoalsList();
                ActionHeuristicManager ahm = new ActionHeuristicManager(person.townieInformation, ws);

                chosenActions[person]= ahm.ChooseBestAction();
            }
            
            ChosenAction action = chosenActions[person];

            WeightedAction weightedAction = action.Action;
            List<Outcome> potentialEffects = weightedAction.potentialOutcomes;

            ActionExecutionManager aem = new ActionExecutionManager(person, ws, allPeople);

            ExecutedAction executedAction = aem.ExecuteAction(action);
            if(executedAction != null) {
                executedActions.Add(executedAction);

                chosenActions[person] = null;

            }
        }

        foreach(Townie person in allPeople) {
            ProximityRelationChange(person.townieInformation);
            person.townieInformation.statusEffectTable.Update();
        }

        timeStep.Add(executedActions);

        foreach (ExecutedAction action in executedActions) {
            wsdm.AddEvent(action, i);
        }
        ws.Tick(10);

    }


    void ProximityRelationChange(Person person)
    {
        foreach(string other in from p in ws.registry.GetPeople()
                                where p.location == person.location
                                select p.id) {
            person.relationships.Increase(other, Relationship.RelationType.friendly, UnityEngine.Random.Range(0, .3f));
        }
    }


    public List<Townie> GetTownies()
    {
        return allPeople;
    }
}

