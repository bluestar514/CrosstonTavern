using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldHub : MonoBehaviour
{
    public WorldState ws;

    public List<List<ExecutedAction>> timeStep = new List<List<ExecutedAction>>();

    public LoadWorldData wi;
    public WorldSpaceDisplayManager wsdm;
    public TextAsset actionData;
    public TextAsset locationData;
    public TextAsset featureData;
    public TextAsset peopleData;

    Dictionary<Person, ChosenAction> chosenActions = new Dictionary<Person, ChosenAction>();
    private void Start()
    {
        InitalizeWorld();

        wsdm.AddPeople(new List<Person>(ws.registry.GetPeople()));


        foreach (Person person in ws.registry.GetPeople()) {
            chosenActions.Add(person, null);


            person.goals = person.gm.GetGoalsList();

        }

        for (int i = 0; i < 1; i++) {
            TimeStep();
        }

    }

    public void TimeStep()
    {
        int i = timeStep.Count;
        

        List<ExecutedAction> executedActions = new List<ExecutedAction>();
        foreach (Person person in ws.registry.GetPeople()) {
            

            if(chosenActions[person] == null) {
                ActionHeuristicManager ahm = new ActionHeuristicManager(person, ws);

                //chosenActions[person]= ahm.ChooseAction();
            }
            
            ChosenAction action = chosenActions[person];

            WeightedAction weightedAction = action.Action;
            List<Outcome> potentialEffects = weightedAction.potentialEffects;

            ActionExecutionManager aem = new ActionExecutionManager(ws.registry.GetPerson(weightedAction.ActorId), ws);

            ExecutedAction executedAction = aem.ExecuteAction(action);
            if(executedAction != null) {
                executedActions.Add(executedAction);

                chosenActions[person] = null;

                person.goals = person.gm.GetGoalsList();

            }
        }
        timeStep.Add(executedActions);

        foreach (ExecutedAction action in executedActions) {
            wsdm.AddEvent(action, i);
        }
        ws.Tick(10);

    }

    void InitalizeWorld()
    {
        wi = new LoadWorldData(actionData, locationData, featureData, peopleData);

        ws = wi.BuildWorld();
        
    }

}

