using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;

public class WorldHub : MonoBehaviour
{
    public int simulatedInitialDays = 0;

    public WorldState ws;

    List<Townie> allPeople;

    public List<List<ExecutedAction>> timeStep = new List<List<ExecutedAction>>();

    public WorldSpaceDisplayManager wsdm;
    public WriteSimulationLog logger;

    Dictionary<Townie, ChosenAction> chosenActions = new Dictionary<Townie, ChosenAction>();

    public bool ready = false;

    public int progress = 0;
    public int total = 1;

    private void Awake()
    {
        ws = WorldStateInitializer.GetWorldState();
        ws.id = "main";

        allPeople = WorldStateInitializer.GetTownies(ws, transform); 

    }

    private void Start()
    {
        Debug.Log("WorldHub: Initializing");
        wsdm.AddPeople(allPeople);

        foreach (Townie person in allPeople) {
            if (person.name == "barkeep") continue;
            chosenActions.Add(person, null);

            person.townieInformation.knownGoals = person.gm.GetGoalsList();

        }

        logger.Init(new List<string>(from person in allPeople
                                     select person.Id));

        StartCoroutine(SimulateInitialDays());
    }

    IEnumerator SimulateInitialDays()
    {
        Debug.Log("WorldHub: Begining Initial Simulation");
        progress = 0;

        total = GetTimeOfNextBarScene( new WorldTime(1, 1, simulatedInitialDays, 0, 0)).ConvertToMinuteCount() 
                - ws.Time.ConvertToMinuteCount();

        for (int i = 0; i < simulatedInitialDays; i++) {
            yield return DayStepAsync();
        }


        ready = true;
        Debug.Log("WorldHub: Ready");
    }

    IEnumerator TimeStep()
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

            logger.WriteEvent(executedAction);
            logger.WriteCharacterState(person);

            //person.ws.Tick(30);
        }

        foreach(Townie person in allPeople) {
            //ProximityRelationChange(person.townieInformation); //isn't visible from the bar space so removing it for simplicity's sake
            person.townieInformation.statusEffectTable.Update();
        }

        timeStep.Add(executedActions);

        foreach (ExecutedAction action in executedActions) {
            wsdm.AddEvent(action, i);
            
        }
        ws.Tick(30);
        progress += 30;
        yield return new WaitForEndOfFrame();
    }


    void ProximityRelationChange(Person person)
    {
        foreach(string other in from p in ws.map.GetPeople()
                                where p.location == person.location
                                select p.id) {
            person.relationships.Increase(other, Relationship.Axis.friendly, UnityEngine.Random.Range(0, .3f));
        }
    }


    public List<Townie> GetTownies()
    {
        return allPeople;
    }

    public void DayStep()
    {
        int CurrentMinute = ws.Time.ConvertToMinuteCount();
        int TargetMinute = GetTimeOfNextBarScene(ws.Time).ConvertToMinuteCount();

        progress = 0;
        total = TargetMinute - CurrentMinute;

        StartCoroutine(DayStepAsync());
    }

    IEnumerator DayStepAsync()
    {
        Debug.Log("WorldHub: Begining Day Step");

        if (ws.Time >= WorldTime.Night) {
            yield return TimeStep();
        }

        int x = 0;
        while(ws.Time <= WorldTime.Night) {
            yield return TimeStep();

            x++;
            if (x > 1000) throw new Exception("Time is not advancing correctly during TimeStep, so we never leave this loop.");
        }

        Debug.Log("WorldHub: Ending Day Step");
    }

    WorldTime GetTimeOfNextBarScene(WorldTime initialTime)
    {

        WorldTime time = new WorldTime(initialTime); 

        if (time >= WorldTime.Night) {
            time.AdvanceToStartOfDay();
        }

        int x = 0;
        while (time <= WorldTime.Night) {
            time.Tick(30);

            x++;
            if (x > 1000) throw new Exception("Time is not advancing correctly during TimeStep, so we never leave this loop.");
        }

        return time;
    }

    public void NewDay()
    {
        int advancedMin = ws.NewDay();
        progress += advancedMin;
        ws.Tick(60 * 8);
        progress += (60 * 8);

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
                                                new Outcome(
                                                    new ChanceModifier(), 
                                                    new List<Effect>(){
                                                        new EffectMovement(person.ws.id, person.homeLocation)
                                                    }
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



            person.gm.DecrementModuleTimers();
        }
    }
}

