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
    public TextAsset locationData;
    public TextAsset featureData;

    Dictionary<Person, ChosenAction> chosenActions = new Dictionary<Person, ChosenAction>();
    private void Start()
    {
        InitalizeWorld();

        wsdm.AddPeople(new List<Person>(ws.registry.GetPeople()));


        foreach (Person person in ws.registry.GetPeople()) {
            chosenActions.Add(person, null);

            person.goalPriorityDict = new GoalDictionary();
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

                List<WeightedAction> weightedActions = ahm.GenerateWeightedOptions();

                chosenActions[person]= ahm.ChooseAction(weightedActions);
            }
            
            ChosenAction action = chosenActions[person];

            WeightedAction weightedAction = action.Action;
            List<Effect> potentialEffects = weightedAction.potentialEffects;

            ActionExecutionManager aem = new ActionExecutionManager(ws.registry.GetPerson(weightedAction.ActorId), ws);

            ExecutedAction executedAction = aem.ExecuteAction(action);
            if(executedAction != null) {
                executedActions.Add(executedAction);

                chosenActions[person] = null;

                person.goals = person.gm.GetGoalsList();

                person.goalPriorityDict = new GoalDictionary();
                foreach (Goal goal in person.goals) {
                    if(!person.goalPriorityDict.ContainsKey(goal.state))
                        person.goalPriorityDict.Add(goal.state, goal.priority);
                }
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
        wi = new LoadWorldData(locationData, featureData); //new WorldInitializer();
        List <Person> people = wi.InitializePeople();
        Registry registry = wi.InitializeRegistry(people);
        Map map = wi.InitializeMap(people);

        ws = new WorldState(map, registry, WorldTime.DayZeroEightAM);


        Person person = ws.registry.GetPerson("Alicia");
        person.placeOfWork = "town_fishShop";
        person.gm = new GoalManager(ws, person);
        person.gm.AddModule(new GM_ManLocation(
                                    new List<GM_Precondition>() {
                                        new GM_Precondition_Time(new WorldTime(-1, -1, -1, 8, 30), new WorldTime(-1, -1, -1, 10, 50))
                                    },
                                    new List<Goal>() { }, 
                                    ws.map.GetFeature(person.placeOfWork).location));
        person.gm.AddModule(new GM_StockFeature(
                                    new List<GM_Precondition>() {
                                        new GM_Precondition_Time(new WorldTime(-1, -1, -1, 11, 0), new WorldTime(-1, -1, -1, 23, 0))
                                    },
                                    new List<Goal>() { }, 
                                    person.Id,
                                    person.placeOfWork, ws));
    }

}

