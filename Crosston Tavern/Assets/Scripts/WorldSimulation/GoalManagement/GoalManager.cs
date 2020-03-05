using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalManager 
{
    WorldState ws;
    Person actor;
    List<GoalModule> modules = new List<GoalModule>();

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
        for(int i =0; i< 5; i++) {
            newGoals = UnravelCausality(newGoals);

            if (newGoals.Count <= 0) break;

            currentGoals.AddRange(newGoals);
        }

        return CondenseGoals( currentGoals);
    }

    List<BoundAction> GetAllActions()
    {
        List<BoundAction> lba = new List<BoundAction>();
        foreach(string location in ws.map.GetNameOfLocations()) {
            lba.AddRange(ws.map.GatherProvidedActionsForActorAt(actor.Id, location));
        }

        return lba;
    } 
   
    List<BoundAction> GetInvalidActions()
    {
        List<BoundAction> allActions = GetAllActions();

        return new List<BoundAction>(from action in allActions
                                           where (!action.SatisfiedPreconditions(ws) || action.LocationId != actor.location) &&
                                                    action.Id != "move"
                                           select action);
    }

    List<BoundAction> GetActionsThatAdvanceState(MicroEffect state, List<BoundAction> actionPool)
    {
        ActionHeuristicManager ahm = new ActionHeuristicManager(actor, ws);

        List<BoundAction> goodActions = new List<BoundAction>();

        foreach (BoundAction action in actionPool) {
            List<Effect> potentialEffects = action.GenerateExpectedEffects(ws);

            foreach(Effect effect in potentialEffects) {
                if( effect.effects.Any(subeffect => ahm.EvaluateEffectTowardGoal(subeffect, state, 0) > 0)) {
                    goodActions.Add(action);
                    break;
                }
            }
        }

        return goodActions;
    }

    List<Goal> GenerateGoalsFromPreconditions(List<BoundAction> invalidActions, float parentPriority)
    {
        List<Goal> newGoals = new List<Goal>();

        foreach(BoundAction action in invalidActions) {
            foreach(Condition condition in action.GetBoundConditions(ws)) {
                if (condition.InEffect(actor, ws.map.GetFeature(action.FeatureId), ws.map.GetLocation(action.LocationId), ws)) continue;

                if(condition is Condition_IsState) {
                    Condition_IsState stateCondition = (Condition_IsState)condition;

                    Goal g = new Goal(stateCondition.state, parentPriority, 1);
                    g.enablingActions.Add(action);

                    newGoals.Add(g);
                }

            }
            Goal goal = new Goal(new Move(action.LocationId), parentPriority, 1);
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

        invalidActions.ForEach(Debug.Log);

        List<Goal> newGoals = new List<Goal>();
        foreach (Goal goal in currentGoals) {
            List<BoundAction> relevantActions = GetActionsThatAdvanceState(goal.state, invalidActions);

            List<Goal> fromThisGoal = GenerateGoalsFromPreconditions(relevantActions, goal.priority);
            fromThisGoal.ForEach(g => g.parentGoals.Add(goal));

            newGoals.AddRange(fromThisGoal);
        }

        return CondenseGoals(newGoals);
    }
}
