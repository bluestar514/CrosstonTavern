using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldHub : MonoBehaviour
{
    public int simulatedInitialDays = 0;

    public WorldState ws;

    List<Townie> allPeople;

    public List<List<ExecutedAction>> timeStep = new List<List<ExecutedAction>>();

    public WorldSpaceDisplayManager wsdm;

    Dictionary<Townie, ChosenAction> chosenActions = new Dictionary<Townie, ChosenAction>();
    private void Awake()
    {
        ws = WorldStateInitializer.GetWorldState();
        ws.id = "main";

        allPeople = WorldStateInitializer.GetTownies(ws, transform); 

    }

    private void Start()
    {
        wsdm.AddPeople(new List<Person>(ws.registry.GetPeople()));

        foreach (Townie person in allPeople) {
            if (person.name == "barkeep") continue;
            chosenActions.Add(person, null);

            person.townieInformation.knownGoals = person.gm.GetGoalsList();

        }

        for (int i = 0; i < simulatedInitialDays; i++) {
            DayStep();
        }
    }

    public void TimeStep()
    {
        if (ws.Time > WorldTime.Night) {
            NewDay();
            
        }

        int i = timeStep.Count;

        
        List<ExecutedAction> executedActions = new List<ExecutedAction>();
        foreach (Townie person in allPeople) {
            if (person.name == "barkeep") continue;

            if (chosenActions[person] == null) {
                person.townieInformation.knownGoals = person.gm.GetGoalsList();
                ActionHeuristicManager ahm = new ActionHeuristicManager(person.townieInformation, person.ws);

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

            //person.ws.Tick(30);
        }

        foreach(Townie person in allPeople) {
            ProximityRelationChange(person.townieInformation);
            person.townieInformation.statusEffectTable.Update();
        }

        timeStep.Add(executedActions);

        foreach (ExecutedAction action in executedActions) {
            wsdm.AddEvent(action, i);
        }
        ws.Tick(30);

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

    public void DayStep()
    {
        if (ws.Time >= WorldTime.Night) {
            TimeStep();
        }

        int x = 0;
        while(ws.Time <= WorldTime.Night) {
            TimeStep();

            x++;
            if (x > 1000) throw new Exception("Time is not advancing correctly during TimeStep, so we never leave this loop.");
        }

        
    }

    public void NewDay()
    {
        ws.NewDay();
        ws.Tick(60 * 8);

        foreach (Townie person in allPeople) {
            if (person.townieInformation.id == "barkeep") continue;

            ActionExecutionManager aem = new ActionExecutionManager(person, ws, allPeople);

            ExecutedAction executedAction = aem.ExecuteAction(new ChosenAction(
                new WeightedAction( new BoundAction( 
                                        new GenericAction( 
                                            "teleportToStart", 
                                            0, 
                                            new Precondition(new List<Condition>()), 
                                            new List<Outcome>() {
                                                new Outcome(new ChanceModifier(), new List<Effect>(){ new EffectMovement(person.ws.id, person.homeLocation) },
                                                                new List<VerbilizationEffect>(){ new VerbilizationEffect("SYSTEM TELEPORT") }
                                                )
                                            }, 
                                            new List<BindingPort>(),
                                            new VerbilizationAction("SYSTEM", "SYSTEM")), 
                                        person.ws.id,
                                        "SYSTEM_SYSTEM",
                                        person.homeLocation,
                                        new BoundBindingCollection(new List<BoundBindingPort>()),
                                        new VerbilizationAction("SYSTEM", "SYSTEM")),
                                    -1, 
                                    new List<WeightedAction.WeightRational>()),
                new List<BoundAction>(),
                new List<WeightedAction>())
            );

        }
    }
}

