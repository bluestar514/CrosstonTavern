using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GoalManager 
{
    WorldState ws;
    Person actor;
    [SerializeField]
    List<GoalModule> modules = new List<GoalModule>();

    int lookAhead = 1;

    public GoalManager(WorldState ws, Person actor)
    {
        this.ws = ws;
        this.actor = actor;
    }

    public void AddModule(GoalModule goalModule)
    {
        modules.Add(goalModule);
    }

    public List<Goal> GetGoalsList()
    {
        List<Goal> currentGoals = new List<Goal>();

        foreach(GoalModule module in modules) {
            if (module.Precondtion(ws)) {
                currentGoals.AddRange(module.goals);
            }
        }

        List<Goal> newGoals = currentGoals;
        for (int i =0; i< lookAhead; i++) {
            newGoals = UnravelCausality(newGoals);

            if (newGoals.Count <= 0) break;

            currentGoals.AddRange(newGoals);
        }

        return CondenseGoals( currentGoals);
    }

    List<BoundAction> GetAllActions()
    {
        ActionBuilder ab = new ActionBuilder(ws, actor);

        List<BoundAction> lba = new List<BoundAction>();
        foreach(string location in ws.map.GetNameOfLocations()) {
            lba.AddRange(ab.GetAllActions(location));
        }

        return lba;
    } 
   
    List<BoundAction> GetInvalidActions()
    {
        List<BoundAction> allActions = GetAllActions();

        return new List<BoundAction>( from action in allActions
                                       where (!action.preconditions.Valid(ws, actor, action.Bindings, ws.map.GetFeature(action.FeatureId).relevantResources) || 
                                                action.LocationId != actor.location) &&
                                                action.Id != "move"
                                       select action);
    }

    IEnumerable<WeightedAction> GetActionsThatAdvanceState(State state, List<BoundAction> actionPool)
    {
        ActionHeuristicManager ahm = new ActionHeuristicManager(actor, ws);

        List<WeightedAction> weightedActions = ahm.WeighActions(actionPool, false);

        return from action in weightedActions
               where action.weight > 0
               select action;
    }

    List<Goal> GenerateGoalsFromPreconditions(List<WeightedAction> invalidActions, float parentPriority)
    {
        List<Goal> newGoals = new List<Goal>();

        foreach (WeightedAction action in invalidActions) {
            BoundBindingCollection bindings = action.Bindings;
            FeatureResources featureResources = ws.map.GetFeature(action.FeatureId).relevantResources;
            foreach (Condition condition in action.preconditions.conditions) {
                if (condition.InEffect(actor, ws, bindings, featureResources)) continue;

                if (condition is Condition_IsState) {
                    Condition_IsState stateCondition = (Condition_IsState)condition;

                    Goal g = new Goal(stateCondition.state.Bind(bindings, featureResources), parentPriority);
                    g.enablingActions.Add(action);

                    newGoals.Add(g);
                }

            }



            Goal goal = new Goal(new StatePosition( bindings.BindString(actor.id), bindings.BindString( action.LocationId)), parentPriority);
            goal.enablingActions.Add(action);

            newGoals.Add(goal);
        }

        return newGoals;
    }


    List<Goal> CondenseGoals(List<Goal> goals)
    {
        List<Goal> condensedGoals = new List<Goal>();

        foreach (Goal goal in goals) {
            Goal sameDomainGoal = condensedGoals.Find(g => g.OperatingOnSameState(goal));
            if (sameDomainGoal == null) condensedGoals.Add(goal);
            else {
                condensedGoals.Add(goal.CondenseGoal(sameDomainGoal));
                condensedGoals.Remove(sameDomainGoal);
            }
        }

        return condensedGoals;
    }


    List<Goal> UnravelCausality(List<Goal> currentGoals)
    {
        List<BoundAction> invalidActions = GetInvalidActions();
        //invalidActions.ForEach(Debug.Log);

        List<Goal> newGoals = new List<Goal>();
        foreach (Goal goal in currentGoals) {
            List<WeightedAction> relevantActions = new List<WeightedAction>( GetActionsThatAdvanceState(goal.state, invalidActions));

            Debug.Log(string.Join(",", relevantActions));

            List<Goal> fromThisGoal = GenerateGoalsFromPreconditions(relevantActions, goal.priority);
            //fromThisGoal.ForEach(g => g.parentGoals.Add(goal));

            newGoals.AddRange(fromThisGoal);
        }

        return CondenseGoals(newGoals);
    }
}
