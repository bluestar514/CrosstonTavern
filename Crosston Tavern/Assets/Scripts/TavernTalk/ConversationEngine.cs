using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationEngine
{
    public Townie townie;
    List<SocialMove> socialMoves;
    string partner;
    
    

    public ConversationEngine(Townie patron, List<SocialMove> socialMoves, string partner)
    {
        townie = patron;
        this.socialMoves = socialMoves;
        this.partner = partner;
        
    }


    public SocialMove GiveResponse(SocialMove prompt)
    {
        List<ExecutedAction> history;
        switch (prompt.verb) {
            case "askAboutGoals":
                return new SocialMove("tellAboutGoals", mentionedFacts: MakeGoalsFacts(townie.gm.GetGoalsList()));
            case "askAboutDayFull":
                List<ExecutedAction> historyFull = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutDayEvents", mentionedFacts: MakeActionFacts(historyFull));
            case "askAboutDayHighlights":
                history = FilterMyActions(GetDayEvents());
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Count > 0
                                                   select e);
                history = CondenseEvents(history);


                return new SocialMove("tellAboutExcitingEvent", mentionedFacts: MakeActionFacts(history));
            case "askAboutObservation":
                List<ExecutedAction> observedHistory = FilterOtherActions(GetDayEvents());
                return new SocialMove("tellAboutDayObservedEvents", mentionedFacts: MakeActionFacts(observedHistory));
            case "askWhyGoal#":
                string goalName = prompt.arguements[0];
                WorldFactGoal goalFact = (WorldFactGoal)prompt.mentionedFacts[0];
                Goal goal = goalFact.goal;

                List<Goal> parentGoals = goal.GetParentGoals();

                return new SocialMove("tellWhyGoal#", new List<string>() { goalName }, mentionedFacts: MakeGoalsFacts(parentGoals));
            case "askWhyAction#":
                string actionName = prompt.arguements[0];
                ExecutedAction action = townie.ws.knownFacts.GetActionFromName(actionName);
                if (action == null) return new SocialMove("dontKnow#", prompt.arguements);
                return new SocialMove("tellWhyAction#", new List<string>() { actionName },
                                                mentionedFacts: MakeGoalsFacts(new List<Goal>(from rational in action.Action.weightRationals
                                                                               select rational.goal)));
            case "askAboutExcitement":
                history = FilterMyActions(GetDayEvents());
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Contains(Opinion.Tag.excited)
                                                    select e);
                history = CondenseEvents(history);

                return new SocialMove("tellAboutExcitingEvent", mentionedFacts: MakeActionFacts(history));
            case "askAboutDisapointment":
                history = FilterMyActions(GetDayEvents());
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Contains(Opinion.Tag.disapointed)
                                                   select e);
                history = CondenseEvents(history);

                return new SocialMove("tellAboutDisapointingEvent", mentionedFacts: MakeActionFacts(history));
            case "tellAction#":
                return new SocialMove("acknowledge", mentionedFacts: prompt.mentionedFacts);
            case "askAboutAction#":
                return new SocialMove("tellDetailsOfAction#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);
            default:
                return new SocialMove("DEFAULT");
        }
    }

    public List<SocialMove> GetSocialMoves()
    {
        List<SocialMove> moves = new List<SocialMove>(socialMoves);
        moves.AddRange(GenAskWhyGoal());
        moves.AddRange(GenAskWhyAction());
        moves.AddRange(GenTellAction());
        //moves.AddRange(GenAskAboutAction());
        return moves;
    }


    public void LearnFromInput(SocialMove prompt)
    {
        foreach(WorldFact fact in prompt.mentionedFacts) {
            townie.ws.LearnFact(fact);
        }
    }

    List<SocialMove> GenAskWhyGoal()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactGoal
                                    select new SocialMove("askWhyGoal#", new List<string> { ((WorldFactGoal)fact).goal.name}, 
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenAskWhyAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == partner //I may take out this condition in the long run
                                    select new SocialMove("askWhyAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                           mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenTellAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId != partner
                                    select new SocialMove("tellAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenAskAboutAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == partner
                                    select new SocialMove("askAboutAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<ExecutedAction> GetDayEvents()
    {
        List<ExecutedAction> completeHistory = townie.ws.knownFacts.GetHistory();
        List<ExecutedAction> todaysHistory = completeHistory.FindAll(x => x.executionTime.SameDay(townie.ws.Time));

        return todaysHistory;
    }

    List<ExecutedAction> FilterMyActions(List<ExecutedAction> todaysHistory)
    {
        List<ExecutedAction> filteredHistory = todaysHistory.FindAll(x => x.Action.ActorId == townie.townieInformation.id ||
                                                                            x.Action.FeatureId == townie.townieInformation.id);

        return filteredHistory;
    }

    List<ExecutedAction> FilterOtherActions(List<ExecutedAction> observedActions)
    {
        List<ExecutedAction> filteredHistory = observedActions.FindAll(x => x.Action.ActorId != townie.townieInformation.id &&
                                                                            x.Action.FeatureId != townie.townieInformation.id);

        return filteredHistory;
    }


    List<WorldFact> MakeGoalsFacts(List<Goal> goals)
    {
        List<WorldFact> facts = new List<WorldFact>();
        string owner = townie.townieInformation.id;
        foreach(Goal goal in goals ) {
            facts.Add(new WorldFactGoal(goal, owner));
        }

        return facts;
    }

    List<WorldFact> MakeActionFacts(List<ExecutedAction> actions)
    {
        List<WorldFact> facts = new List<WorldFact>();
        foreach (ExecutedAction action in actions) {
           facts.Add(new WorldFactEvent(action));
        }

        return facts;
    }

    List<ExecutedAction> CondenseEvents(List<ExecutedAction> fullList)
    {
        int count = 0;
        int initialCount = fullList.Count;

        List<ExecutedAction> condensed = new List<ExecutedAction>();
        while (fullList.Count > 0) {
            ExecutedAction action = fullList[0];
            fullList.RemoveAt(0);
            count++;
            for (int i = fullList.Count - 1; i >= 0; i--) {
                ExecutedAction combination = CondenseTwoEvents(action, fullList[i]);
                if (combination != null) {
                    action = combination;
                    fullList.RemoveAt(i);
                    count++;
                }
            }

            condensed.Add(action);
        }
        return condensed;
    }

    /// <summary>
    /// Returns null if two actions are not the same kind of action
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <returns></returns>
    ExecutedAction CondenseTwoEvents(ExecutedAction a1, ExecutedAction a2)
    {
        ExecutedAction executedAction = null;
        WeightedAction initial = a1.Action;
        WeightedAction comparison = a2.Action;
        if (initial.Id == comparison.Id &&
            initial.ActorId == comparison.ActorId &&
            initial.FeatureId == comparison.FeatureId) {

            List<Effect> combinedEffects = new List<Effect>(a1.executedEffect);
            combinedEffects.AddRange(a2.executedEffect);
            combinedEffects = CondenseEffects(combinedEffects);

            executedAction = a1.ShallowCopy();
            executedAction.executedEffect = combinedEffects;
        }

        return executedAction;
    }

    List<Effect> CondenseEffects(List<Effect> fullList)
    {
        List<Effect> condensed = new List<Effect>();
        while (fullList.Count > 0) {
            Effect effect = fullList[0];
            fullList.RemoveAt(0);
            for (int i = fullList.Count - 1; i >= 0; i--) {
                Effect combination = effect.Combine(fullList[i]);
                if (combination != null) {
                    effect = combination;
                    fullList.RemoveAt(i);
                }
            }

            condensed.Add(effect);
        }

        return condensed;
    }
}
