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
        }

        ws.registry.GetPerson("Alicia").placeOfWork = "town_fishShop";

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


                GoalManager gm = new GoalManager(ws, person);
                gm.AddModule(new GoalModule(person, new List<Goal>() {
                    new Goal(new InvChange(1, 1, person.Id, new List<string>(){"salmon","trout"}), 1, 1)
                }));

                person.goals = gm.GetGoalsList();

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
    }

}

