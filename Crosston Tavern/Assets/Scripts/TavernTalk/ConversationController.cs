using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationController 
{
    Townie townie;
    List<SocialMove> socialMoves; 

    public ConversationController(Townie patron, List<SocialMove> socialMoves)
    {
        townie = patron;
        this.socialMoves = socialMoves;
    }


    public SocialMove GiveResponse(SocialMove prompt)
    {

        switch (prompt.verb) {
            case "askAboutGoals":
                return new SocialMove("tellAboutGoals", mentionedGoals: GetGoals());
            case "askAboutDayFull":
                List<ExecutedAction> historyFull = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutDayEvents", mentionedActions: historyFull);
            case "askAboutDayHighlights":
                List<ExecutedAction> history = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutExcitingEvent", mentionedActions: new List<ExecutedAction>(from e in history
                                                                                                           where e.opinion.tags.Count > 0
                                                                                                           select e));
            case "askAboutObservation":
                List<ExecutedAction> observedHistory = FilterOtherActions(GetDayEvents());
                return new SocialMove("tellAboutDayObservedEvents", mentionedActions: observedHistory);
            case "askWhyGoal#":
                string goalName = prompt.arguements[0];
                Goal goal = townie.gm.GetGoalFromName(goalName);

                /*Debug.Log(goal);
                Debug.Log(goal.parentGoals.Count);*/

                List<Goal> parentGoals = new List<Goal>(from g in goal.parentGoals
                                                        select townie.gm.GetGoalFromName(g));

                return new SocialMove("tellWhyGoal#", new List<string>() { goalName }, mentionedGoals: parentGoals);
            case "askWhyAction#":
                string actionName = prompt.arguements[0];
                ExecutedAction action = townie.ws.knownFacts.GetActionFromName(actionName);
                if (action == null) return new SocialMove("dontKnow#", prompt.arguements);
                return new SocialMove("tellWhyAction#", new List<string>() { actionName }, string.Join(",", action.Action.weightRationals));/*,
                                                mentionedGoals: new List<Goal>(from rational in action.Action.weightRationals
                                                                               select new Goal(rational.goal, 1)));*/
            case "askAboutExcitement":
                List<ExecutedAction> fullHistory = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutExcitingEvent", mentionedActions: new List<ExecutedAction>(from e in fullHistory
                                                                                                           where e.opinion.tags.Contains(Opinion.Tag.excited)
                                                                                                           select e));
            case "askAboutDisapointment":
                List<ExecutedAction> fullHistory2 = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutExcitingEvent", mentionedActions: new List<ExecutedAction>(from e in fullHistory2
                                                                                                           where e.opinion.tags.Contains(Opinion.Tag.disapointed)
                                                                                                           select e));
            case "tellAction#":
                return new SocialMove("acknowledge");
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
        return moves;
    }


    public void LearnFromInput(SocialMove prompt)
    {
        foreach(WorldFact fact in prompt.mentionedFacts) {
            townie.ws.LearnFact(fact);
        }
    }

    public DialogueUnit ExpressSocialMove(SocialMove socialMove)
    {
        return new DialogueUnit(socialMove.ToString(), townie.name, socialMove);
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
                                    select new SocialMove("askWhyAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                           mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenTellAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    select new SocialMove("tellAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<Goal> GetGoals()
    {
        return townie.gm.GetGoalsList();
    }

    List<ExecutedAction> GetDayEvents()
    {
        List<ExecutedAction> completeHistory = townie.history;
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

}
