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
    public List<Goal> lastSetOfGoals = new List<Goal>();

    int lookAhead = 3;

    public GoalManager(WorldState ws, Person actor)
    {
        this.ws = ws;
        this.actor = actor;
    }

    public void AddModule(GoalModule goalModule)
    {
        modules.Add(goalModule);
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
                List<OutcomeRestraints> relevantOutcomes = FilterOutcomesThatFullfillGoal(goal, allOutcomes);
                newGoals.AddRange(MakeGoalsFromOutcome(goal, relevantOutcomes));
            }

            List<Goal> condensedGoals = CondenseGoals(newGoals);

            allGoals.AddRange(condensedGoals);
            previousIterationGoals = new List<Goal>(condensedGoals);
            newGoals = new List<Goal>();
        }

        allGoals = CondenseGoals(allGoals);

        allGoals = FindLocations(allGoals);


        lastSetOfGoals = allGoals;
        return allGoals;
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
    }


    List<BoundAction> GetAllActions()
    {
        ActionBuilder ab = new ActionBuilder(ws, actor);

        List<BoundAction> lba = new List<BoundAction>();
        foreach (string location in ws.map.GetNameOfLocations()) {
            lba.AddRange(ab.GetAllActions(location));
        }

        return lba;
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
            if (OutcomeProgressesGoal(outcome, goal)) {
                outcome.fullfillsGoal.Add(goal);
                relevantOutcomes.Add(outcome);
            }
        }

        return relevantOutcomes;
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

    List<Goal> MakeGoalsFromOutcome(Goal goal, List<OutcomeRestraints> relevantOutcomes)
    {
        List<Goal> newGoals = new List<Goal>();
        foreach (OutcomeRestraints outcome in relevantOutcomes) {
            if (goal.enablingActions.Contains(outcome.parentAction)) continue;

            float effectStrength = outcome.effects.Sum(effect => effect.WeighAgainstGoal(ws,
                                                                                        outcome.bindings,
                                                                                        outcome.resources,
                                                                                        goal));

            newGoals.AddRange(MakePreconditionsGoal(outcome,
                                                    goal.priority *
                                                        outcome.chanceModifier.MakeBound(outcome.bindings, outcome.resources).Chance(ws) *
                                                        effectStrength
                                                    ));
            newGoals.AddRange(MakeChanceModifierGoal(outcome, goal.priority * effectStrength));
        }

        return newGoals;
    }

    List<Goal> MakePreconditionsGoal(OutcomeRestraints outcome, float parentPriority)
    {
        List<Goal> newGoals = new List<Goal>();

        foreach(Condition condition in outcome.preconditions) {
            if (condition.InEffect(actor, ws, outcome.bindings, outcome.resources)) continue;

            if (condition is Condition_IsState) {
                Condition_IsState stateCondition = (Condition_IsState)condition;

                Goal g = new Goal(stateCondition.state.Bind(outcome.bindings, outcome.resources), parentPriority);
                g.enablingActions.Add(outcome.parentAction);

                foreach(Goal goal in outcome.fullfillsGoal) {
                    g.AddParentGoal(goal);
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
                goal.enablingActions.Add(outcome.parentAction);
                foreach (Goal g in outcome.fullfillsGoal) {
                    goal.AddParentGoal(g);
                }
            }
        }

        return newGoals;
    }
 
    List<Goal> CondenseGoals(List<Goal> goals)
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

        List<BoundAction> allActions = GetAllActions();
        ActionHeuristicManager ahm = new ActionHeuristicManager(actor, ws);
        
        foreach (BoundAction action in allActions) {
            if (action.preconditions.Valid(ws, actor, action.Bindings, ws.map.GetFeature(action.FeatureId).relevantResources)) {
                float desire = ahm.WeighActions(new List<BoundAction>() { action })[0].weight;
                desire = Mathf.Max(0, desire);

                List<OutcomeRestraints> outcomes = DecomposeActionToOutcomes(action);
                foreach (OutcomeRestraints outcome in outcomes) {
                    foreach (Goal goal in goals) {
                        if (OutcomeProgressesGoal(outcome, goal)) {
                            string location = action.LocationId;
                            if (locationPriority.ContainsKey(location)) {
                                locationPriority[location] += desire;
                                locationReason[location].Add(goal);
                            } else {
                                locationPriority.Add(location, desire);
                                locationReason.Add(location, new List<Goal>() { goal });
                            }
                        }

                    }
                }
            }
        }


        foreach(string location in locationPriority.Keys) {
            Goal goal = new Goal(new StatePosition(actor.id, location), locationPriority[location]);
            
            foreach(Goal g in locationReason[location]) {
                goal.AddParentGoal(g);
            }

            goals.Add(goal);
        }

        return goals;
    }
}
