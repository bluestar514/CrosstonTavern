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
    List<Goal> lastSetOfGoals = new List<Goal>(); //only for use in GM, for outside GM use the Townie Information's KnownGoals

    int lookAhead = 7;

    public enum GoalPriority
    {
        low = 1,
        medium = 3,
        high = 5,
        imperitive = 10
    }

    public GoalManager(WorldState ws, Person actor)
    {
        this.ws = ws;
        this.actor = actor;
    }

    public void AddModule(GoalModule goalModule)
    {
        modules.Add(goalModule);
    }

    public void DecrementModuleTimers()
    {

        List<GoalModule> deactivatedModules = new List<GoalModule>();
        foreach(GoalModule gm in modules) {
            gm.DecrementTimer();

            if (gm.timer == 0) deactivatedModules.Add(gm);
        }

        foreach(GoalModule gm in deactivatedModules) {
            modules.Remove(gm);
        }
    }

    public Goal GetGoalFromName(string name)
    {
        foreach(Goal goal in lastSetOfGoals) {
            if (goal.name == name) return goal;
        }

        return null;
    }

    public List<Goal> GetGoalsList()
    {
        // Get all actions
        List<BoundAction> allActions = GetAllActions();
        // Get all outcomes
        List<OutcomeRestraints> allOutcomes = new List<OutcomeRestraints>();
        foreach(BoundAction action in allActions) {
            allOutcomes.AddRange(DecomposeActionToOutcomes(action));
        }

        // Get High level goals:
        List<Goal> initialGoals = new List<Goal>();

        List<GoalModule> allModules = new List<GoalModule>(modules);
        foreach(Obligation ob in actor.schedule.obligations) {

            allModules.Add(ob.gMod);
        }

        foreach (GoalModule module in allModules) {
            if (module.Precondtion(ws)) {
                initialGoals.AddRange(module.goals);
            }
        }

        // For x iterations:
        List<Goal> allGoals = new List<Goal>(initialGoals);
        List<Goal> previousIterationGoals = new List<Goal>(initialGoals);
        List<Goal> newGoals = new List<Goal>();


        for (int i= 0; i<lookAhead; i++) {
            
            // Foreach goal, grab relevant set of outcomes and their restraints
            foreach (Goal goal in previousIterationGoals) {
                if (goal is GoalState) {
                    List<OutcomeRestraints> relevantOutcomes = FilterOutcomesThatFullfillGoal(goal, allOutcomes);
                    newGoals.AddRange(MakeGoalsFromOutcome(goal, relevantOutcomes));
                }else if(goal is GoalAction goalAction) {
                    newGoals.AddRange(MakeGoalFromAction(goalAction, allActions));
                }
            }

            List<Goal> condensedGoals = CondenseGoals(newGoals);

            allGoals.AddRange(condensedGoals);
            previousIterationGoals = new List<Goal>(condensedGoals);
            newGoals = new List<Goal>();
        }

        allGoals = CondenseGoals(allGoals);
        allGoals = FindLocations(allGoals);
        allGoals = CondenseGoals(allGoals);

        lastSetOfGoals = allGoals;
        return allGoals;
    }

    public List<GoalModule> GetParentModule(Goal childGoal)
    {
        List<GoalModule> parentModules = new List<GoalModule>();

        foreach(GoalModule module in modules) {
            if (!module.Precondtion(ws)) continue;

            foreach(Goal goal in module.goals) {
                if (goal.Equals(childGoal)) parentModules.Add( module);
            }
        }

        return parentModules;
    }

    public List<Goal> GetStuckGoals()
    {
        
        List<BoundAction> allActions = GetAllActions();
        // Get all outcomes
        List<OutcomeRestraints> allOutcomes = new List<OutcomeRestraints>();
        foreach (BoundAction action in allActions) {
            allOutcomes.AddRange(DecomposeActionToOutcomes(action));
        }

        
        List<Goal> goals = lastSetOfGoals;

        List<Goal> stuckGoals = new List<Goal>();
        foreach(Goal goal in goals) {
            List<OutcomeRestraints> positiveOutcomes = new List<OutcomeRestraints>();
            if (goal is GoalState) {
                positiveOutcomes = FilterOutcomesThatFullfillGoal(goal, allOutcomes);
                
            } else if (goal is GoalAction goalAction) {
                positiveOutcomes.AddRange(OutcomeRestraints.MakeFromAction(goalAction.action, ws));
            }

            //mark goals which have no known actions 
            if (positiveOutcomes.Count == 0) stuckGoals.Add(goal);
        }

        return stuckGoals;
    }

    class OutcomeRestraints
    {
        public List<Effect> effects;
        public ChanceModifier chanceModifier;
        public List<Condition> preconditions;

        public BoundBindingCollection bindings;
        public FeatureResources resources;

        public BoundAction parentAction;
        public List<Goal> fullfillsGoal;

        public override string ToString()
        {
            return "<"+parentAction+"{"+string.Join(",", effects)+"} {" +bindings+ ", "+ resources+"} "+chanceModifier+" {"+string.Join(",", preconditions)+"}>";
        }
        public OutcomeRestraints(List<Effect> effects, ChanceModifier chanceModifier, List<Condition> preconditions, BoundBindingCollection bindings, FeatureResources resources, BoundAction parentAction)
        {
            this.effects = effects;
            this.chanceModifier = chanceModifier;
            this.preconditions = preconditions;

            this.bindings = bindings;
            this.resources = resources;
            this.parentAction = parentAction;
            fullfillsGoal = new List<Goal>();
        }

        public static List<OutcomeRestraints> MakeFromAction(BoundAction parentAction, WorldState ws)
        {
            return new List<OutcomeRestraints>(from outcome in parentAction.potentialOutcomes
                                               select new OutcomeRestraints(outcome.effects,
                                                                            outcome.chanceModifier,
                                                                            parentAction.preconditions.conditions,
                                                                            parentAction.Bindings,
                                                                            ws.map.GetFeature(parentAction.FeatureId).relevantResources,
                                                                            parentAction));
        }
    }


    List<BoundAction> GetAllActions()
    {
        ActionBuilder ab = new ActionBuilder(ws, actor);

        return ab.GetAllActions(respectLocation:false);
    }

    List<OutcomeRestraints> DecomposeActionToOutcomes(BoundAction action)
    {
        List<OutcomeRestraints> outcomeRestraints = new List<OutcomeRestraints>();

        foreach(Outcome outcome in action.potentialOutcomes) {
            List<Effect> effects = outcome.effects;
            ChanceModifier chanceModifier = outcome.chanceModifier;
            List<Condition> preconditions = action.preconditions.conditions;

            BoundBindingCollection bindings = action.Bindings;
            FeatureResources resources = ws.map.GetFeature(action.FeatureId).relevantResources;

            OutcomeRestraints restraints = new OutcomeRestraints(effects, chanceModifier, preconditions, bindings, resources, action);
            outcomeRestraints.Add(restraints);
        }

        return outcomeRestraints;
    }

    List<OutcomeRestraints> FilterOutcomesThatFullfillGoal(Goal goal, List<OutcomeRestraints> outcomes)
    {
        List<OutcomeRestraints> relevantOutcomes = new List<OutcomeRestraints>();
        foreach(OutcomeRestraints outcome in outcomes) {
            if (ContainsNonActionableConditions(outcome.preconditions, outcome.bindings, actor.id)) continue; 

            if (OutcomeProgressesGoal(outcome, goal) && !OutcomeHurtsParentGoals(outcome, goal)) {
                outcome.fullfillsGoal.Add(goal);
                relevantOutcomes.Add(outcome);
            }
        }

        return relevantOutcomes;
    }

    static public bool ContainsNonActionableConditions(List<Condition> preconditions, BoundBindingCollection bindings, string actor)
    {
        return ContainsConditionNotYou(preconditions, bindings, actor) ||
                ContainsOthersInventoryState(preconditions, bindings, actor) ||
                ContainsConditionItemClass(preconditions, bindings) ||
                ContainsConditionIsYou(preconditions, bindings, actor);

    }

    static bool ContainsOthersInventoryState(List<Condition> preconditions, BoundBindingCollection bindings, string actor)
    {
        foreach (Condition condition in preconditions) {
            if (condition is Condition_IsState) {
                Condition_IsState condition_IsState = (Condition_IsState)condition;

                State state = condition_IsState.state;
                if (state is StateInventory) {
                    StateInventory stateInventory = (StateInventory)state;
                    string owner = bindings.BindString(stateInventory.ownerId);

                    if (owner != actor) { 
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static bool ContainsConditionNotYou(List<Condition> preconditions, BoundBindingCollection bindings, string actor)
    {
        foreach (Condition condition in preconditions) {
            if (condition is Condition_NotYou) {
                Condition_NotYou condition_NotYou = (Condition_NotYou)condition;
                
                if (bindings.BindString(condition_NotYou.featureId) == actor) { //this can never change, so we should just discard this action
                    return true;
                }
            }
        }

        return false;
    }

    static bool ContainsConditionIsYou(List<Condition> preconditions, BoundBindingCollection bindings, string actor)
    {
        foreach (Condition condition in preconditions) {
            if (condition is Condition_IsYou) {
                Condition_IsYou condition_IsYou = (Condition_IsYou)condition;
                if (bindings.BindString(condition_IsYou.featureId) != actor) { //this can never change, so we should just discard this action
                    return true;
                }
            }
        }

        return false;
    }

    static bool ContainsConditionItemClass(List<Condition> preconditions, BoundBindingCollection bindings)
    {
        foreach (Condition condition in preconditions) {
            if (condition is Condition_IsItemClass) {
                Condition_IsItemClass condition_ItemClass = (Condition_IsItemClass)condition;
                string itemId = bindings.BindString(condition_ItemClass.itemId);
                if (!ItemInitializer.IsItem(itemId, condition_ItemClass.itemClass) ) {
                    return true;
                }
            }
        }

        return false;
    }

    bool OutcomeHurtsParentGoals(OutcomeRestraints outcome, Goal goal)
    {
        bool hurtful = OutcomeHurtsGoal(outcome, goal);

        if(goal.parentGoals.Count > 0) {
            foreach(Goal parent in goal.GetParentGoals()) {
                if (hurtful == true) break;
                hurtful = OutcomeHurtsParentGoals(outcome, parent);
            }
        }

        return hurtful;

        //return goal.GetParentGoals().All(parent => !OutcomeHurtsGoal(outcome, parent));
    }

    bool OutcomeHurtsGoal(OutcomeRestraints outcome, Goal goal)
    {
        foreach (Effect effect in outcome.effects) {

            if (effect.WeighAgainstGoal(ws, outcome.bindings, outcome.resources, goal) < 0) {
                return true;
            }
        }

        return false;
    }

    bool OutcomeProgressesGoal(OutcomeRestraints outcome, Goal goal)
    {
        foreach (Effect effect in outcome.effects) {

            if (effect.WeighAgainstGoal(ws, outcome.bindings, outcome.resources, goal) > 0) {
                return true;
            }
        }

        return false;
    }

    List<Goal> MakeGoalsFromOutcome(Goal parentGoal, List<OutcomeRestraints> outcomesFillingParentGoal)
    {
        List<Goal> newGoals = new List<Goal>();
        foreach (OutcomeRestraints outcome in outcomesFillingParentGoal) {
            if (parentGoal.unlockedActionsOnGoalCompletion.Contains(outcome.parentAction)) continue;

            float effectStrength = outcome.effects.Sum(effect => effect.WeighAgainstGoal(ws,
                                                                                        outcome.bindings,
                                                                                        outcome.resources,
                                                                                        parentGoal));
            effectStrength /= outcome.effects.Count;

            newGoals.AddRange(MakePreconditionsGoal(outcome,
                                                    parentGoal.priority *
                                                        outcome.chanceModifier.MakeBound(outcome.bindings, outcome.resources).Chance(ws) *
                                                        effectStrength
                                                    ));
            newGoals.AddRange(MakeChanceModifierGoal(outcome, parentGoal.priority * effectStrength));
        }

        //List<Goal> unloopingGoals = new List<Goal>();
        //foreach(Goal goal in newGoals) {
        //    bool looping = false;
        //    foreach(string parent in goal.parentGoals) {
        //        if (goal.state.ToString() == parent.Split(':')[0]) {
        //            looping = true;
        //        } 
        //    }

        //    if (!looping) {
        //        unloopingGoals.Add(goal);
        //    }

        //}

        return newGoals; //unloopingGoals;
    }

    List<Goal> MakeGoalFromAction(GoalAction parentGoal, List<BoundAction> actions)
    {
        List<Goal> newGoals = new List<Goal>();
        foreach (BoundAction action in actions) {

            if (parentGoal.action.Equals(action)){

                BoundBindingCollection bindings = action.Bindings;
                FeatureResources resources = ws.map.GetFeature(action.FeatureId).relevantResources;

                OutcomeRestraints outcome = new OutcomeRestraints(new List<Effect>(), new ChanceModifierSimple(1),
                                                                   action.preconditions.conditions, bindings, resources, action);
                outcome.fullfillsGoal.Add(parentGoal);

                newGoals.AddRange(MakePreconditionsGoal(outcome, parentGoal.priority ));
            }
        }

        return newGoals;
    }

    List<Goal> MakePreconditionsGoal(OutcomeRestraints outcome, float parentPriority)
    {
        List<Goal> newGoals = new List<Goal>();

        if (parentPriority == 0) return newGoals;

        foreach (Condition condition in outcome.preconditions) {
            if (condition.InEffect(actor, ws, outcome.bindings, outcome.resources)) continue;

            if (condition is Condition_IsState) {
                Condition_IsState stateCondition = (Condition_IsState)condition;

                GoalState g = new GoalState(stateCondition.state.Bind(outcome.bindings, outcome.resources), parentPriority);
                g.AddUnlockedAction(outcome.parentAction);

                foreach(Goal parentGoal in outcome.fullfillsGoal) {
                    g.AddParentGoal(parentGoal);
                }

                newGoals.Add(g);
            }
        }

        return newGoals;
        
    }

    List<Goal> MakeChanceModifierGoal(OutcomeRestraints outcome, float parentPriority)
    {
        List<Goal> newGoals = new List<Goal>();

        ChanceModifier chance = outcome.chanceModifier.MakeBound(outcome.bindings, outcome.resources);

        if (chance.Chance(ws) >= 1) return newGoals;

        List<Goal> goals = chance.MakeGoal(ws, parentPriority);

        if (goals != null) {
            foreach (Goal goal in goals) {
                newGoals.Add(goal);
                goal.AddUnlockedAction(outcome.parentAction);
                foreach (Goal g in outcome.fullfillsGoal) {
                    goal.AddParentGoal(g);
                }
            }
        }

        return newGoals;
    }
 
    public static List<Goal> CondenseGoals(List<Goal> goals)
    {
        List<Goal> condensedGoals = new List<Goal>();

        while(goals.Count > 0) {
            int i = goals.Count - 1;
            Goal a = goals[i];
            goals.RemoveAt(i);
            for (int j= i-1; j >= 0; j--) {
                Goal b = goals[j];

                List<Goal> combined = a.Combine(b);

                if(combined.Count == 1) {
                    goals.RemoveAt(j);
                    a = combined[0];
                }
            }
            condensedGoals.Add(a);

        }

        return condensedGoals;
    }


    List<Goal> FindLocations(List<Goal> goals)
    {
        Dictionary<string, float> locationPriority = new Dictionary<string, float>();
        Dictionary<string, List<Goal>> locationReason = new Dictionary<string, List<Goal>>();
        Dictionary<string, List<BoundAction>> locationActions = new Dictionary<string, List<BoundAction>>();

        List<BoundAction> allActions = GetAllActions();
        ActionHeuristicManager ahm = new ActionHeuristicManager(actor, ws);
        
        foreach (BoundAction action in allActions) {
            if (action.preconditions.Valid(ws, actor, action.Bindings, ws.map.GetFeature(action.FeatureId).relevantResources)) {
                float desire = ahm.GetWeightOfBoundAction(action, goals).weight;
                desire = Mathf.Max(0, desire);

                List<OutcomeRestraints> outcomes = DecomposeActionToOutcomes(action);
                foreach (OutcomeRestraints outcome in outcomes) {
                    foreach (Goal goal in goals) {
                        if ((OutcomeProgressesGoal(outcome, goal) && !OutcomeHurtsParentGoals(outcome, goal))||
                            (goal is GoalAction goalAction && goalAction.action.Equals(action))) {
                            string location = action.LocationId;
                            if (locationPriority.ContainsKey(location)) {
                                locationPriority[location] += desire;
                                locationReason[location].Add(goal);
                                locationActions[location].Add(action);
                            } else {
                                locationPriority.Add(location, desire);
                                locationReason.Add(location, new List<Goal>() { goal });
                                locationActions.Add(location, new List<BoundAction>() { action });
                            }
                        }

                    }
                }
            }
        }


        foreach(string location in locationPriority.Keys) {
            GoalState goal = new GoalState(new StatePosition(actor.id, location), locationPriority[location]);
            
            foreach(Goal g in locationReason[location]) {
                goal.AddParentGoal(g);
            }

            foreach (BoundAction action in locationActions[location]) {
                goal.AddUnlockedAction(action);
            }

            goals.Add(goal);
        }

        return goals;
    }
}
