using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationController 
{
    Patron patron;
    Townie townie;

    public ConversationController(Patron patron)
    {
        this.patron = patron;
        townie = patron.townie;
    }


    public SocialMove GiveResponse(SocialMove prompt)
    {
        switch (prompt.verb) {
            case "askAboutGoals":
                return new SocialMove("tellAboutGoals", mentionedGoals: GetGoals());
            case "askAboutDay":
                List<ExecutedAction> history = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutDayEvents", mentionedActions: history);
            case "askAboutObservation":
                List<ExecutedAction> observedHistory = FilterOtherActions(GetDayEvents());
                return new SocialMove("tellAboutDayObservedEvents", mentionedActions: observedHistory);
            case "askWhyGoal#":
                string goalName = prompt.arguements[0];
                Goal goal = townie.gm.GetGoalFromName(goalName);

                List<Goal> parentGoals = new List<Goal>(from g in goal.parentGoals
                                                        select townie.gm.GetGoalFromName(g));

                return new SocialMove("tellWhyGoal#", new List<string>() { goalName }, mentionedGoals: parentGoals);
            case "askWhyAction#":
                string actionName = prompt.arguements[0];
                ExecutedAction action = townie.ws.knownFacts.GetActionFromName(actionName);
                return new SocialMove("tellWhyAction#", new List<string>() { actionName }, System.String.Join(",", action.Action.weightRationals));
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

            /*case "askAboutGoalFrustration":
            List<Goal> goals = GetGoals();
            foreach (Goal goal in goals) {
                goal.state
            }*/


            default:
                return patron.socialMoves[Mathf.FloorToInt(Random.value*patron.socialMoves.Count)];
        }
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
