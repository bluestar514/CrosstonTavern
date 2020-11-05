using System.Collections;
using System.Collections.Generic;
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
                return new SocialMove("tellAboutGoals", content:System.String.Join(",", GetGoals()));
            case "askAboutDay":
                List<ExecutedAction> history = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutDayEvents", content: System.String.Join(",", history));
            case "askAboutObservation":
                List<ExecutedAction> observedHistory = FilterOtherActions(GetDayEvents());
                return new SocialMove("tellAboutDayObservedEvents", content: System.String.Join(",", observedHistory));
            case "askWhyGoal#":
                string goalName = prompt.arguements[0];
                Goal goal = townie.gm.GetGoalFromName(goalName);
                return new SocialMove("tellWhyGoal#", new List<string>() { goalName }, System.String.Join(",", goal.parentGoals));
            case "askWhyAction#":
                string actionName = prompt.arguements[0];
                ExecutedAction action = townie.ws.knownFacts.GetActionFromName(actionName);
                return new SocialMove("tellWhyAction#", new List<string>() { actionName }, System.String.Join(",", action.Action.weightRationals));
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
        Debug.Log(System.String.Join(",", townie.gm.GetGoalsList()));

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
