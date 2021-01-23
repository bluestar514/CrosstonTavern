using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class SocialMove
{
    public List<string> arguements;
    public string content;

    public List<WorldFact> mentionedFacts;

    public SocialMove(List<string> arguements = null, string content = "",
                         List<WorldFact> mentionedFacts = null)
    {
        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;

        if (mentionedFacts == null) mentionedFacts = new List<WorldFact>();
        this.mentionedFacts = mentionedFacts;

        this.content = content + string.Join(",", mentionedFacts);
        
    }

    public override string ToString()
    {
        string name = base.ToString();

        if (content == "")
            return "<" + name + ">";
        else
            return "<" + name + ":" + content + ">";
    }

    //Virtual:

    /// <summary>
    /// Should be called from inside the constructor, 
    /// uses a lot of the constructor/class's feilds to do its job
    /// </summary>
    protected virtual void FillFacts(Townie speaker)
    {

    }

    public virtual string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return ToString();
    }


    // Helpers:
    protected static List<ExecutedAction> GetDayEvents(Townie speaker)
    {
        List<ExecutedAction> completeHistory = speaker.ws.knownFacts.GetHistory();
        List<ExecutedAction> todaysHistory = completeHistory.FindAll(x => x.executionTime.SameDay(speaker.ws.Time));

        return todaysHistory;
    }
    protected static List<ExecutedAction> FilterMyActions(Townie speaker, List<ExecutedAction> todaysHistory)
    {
        List<ExecutedAction> filteredHistory = todaysHistory.FindAll(x => x.Action.ActorId == speaker.townieInformation.id ||
                                                                            x.Action.FeatureId == speaker.townieInformation.id);

        return filteredHistory;
    }
    protected static List<ExecutedAction> FilterOtherActions(Townie speaker, List<ExecutedAction> observedActions)
    {
        List<ExecutedAction> filteredHistory = observedActions.FindAll(x => x.Action.ActorId != speaker.townieInformation.id &&
                                                                            x.Action.FeatureId != speaker.townieInformation.id);

        return filteredHistory;
    }

    protected static List<WorldFact> MakeGoalsFacts(Townie speaker, List<Goal> goals)
    {
        List<WorldFact> facts = new List<WorldFact>();
        string owner = speaker.townieInformation.id;
        foreach (Goal goal in goals) {
            facts.Add(new WorldFactGoal(goal, owner));
        }

        return facts;
    }
    protected static List<WorldFact> MakeActionFacts(List<ExecutedAction> actions)
    {
        List<WorldFact> facts = new List<WorldFact>();
        foreach (ExecutedAction action in actions) {
            facts.Add(new WorldFactEvent(action));
        }

        return facts;
    }


    protected static List<ExecutedAction> CondenseEvents(List<ExecutedAction> fullList)
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
    protected static ExecutedAction CondenseTwoEvents(ExecutedAction a1, ExecutedAction a2)
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
    protected static List<Effect> CondenseEffects(List<Effect> fullList)
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

    protected string VerbalizeAllEvents(List<WorldFact> facts)
    {
        string verbilization = "";
        List<ExecutedAction> allActions = new List<ExecutedAction>();

        foreach (WorldFact fact in facts) {
            if (fact is WorldFactEvent) {

                WorldFactEvent e = (WorldFactEvent)fact;
                ExecutedAction a = e.action;
                allActions.Add(a);
            }
        }

        List<string> collectedEvents = new List<string>();
        foreach (ExecutedAction action in allActions) {
            collectedEvents.Add(v.VerbalizeActionWithResults(action, false));
        }

        if (collectedEvents.Count >= 3) {
            string lastevent = collectedEvents.Last();
            collectedEvents.RemoveAt(collectedEvents.Count - 1);

            verbilization += string.Join(", ", collectedEvents) + ", and " + lastevent + ".";
        } else if (collectedEvents.Count == 2) {
            verbilization += string.Join(" and ", collectedEvents) + ".";
        } else if (collectedEvents.Count == 1) {
            verbilization += collectedEvents[0];
        } else {
            verbilization = "nothing interesting happened.";
        }

        return verbilization;
    }

    protected string VerbalizeByTimePeriod(List<WorldFact> facts)
    {
        List<WorldFact> morniningEvents = new List<WorldFact>();
        List<WorldFact> afternoonEvents = new List<WorldFact>();
        List<WorldFact> eveningEvents = new List<WorldFact>();

        foreach (WorldFact fact in facts) {
            WorldFactEvent e = (WorldFactEvent)fact;
            ExecutedAction a = e.action;
            WorldTime time = a.executionTime;
            if (time < WorldTime.Noon) {
                morniningEvents.Add(e);
            } else if (time < WorldTime.Evening) {
                afternoonEvents.Add(e);
            } else if (time < WorldTime.Night) {
                eveningEvents.Add(e);
            }
        }

        string verbalization = "";
        if (morniningEvents.Count > 0) verbalization = verbalization + "This morning, " + VerbalizeAllEvents(morniningEvents) + ". ";
        if (afternoonEvents.Count > 0) verbalization = verbalization + "This afternoon, " + VerbalizeAllEvents(morniningEvents) + ". ";
        if (eveningEvents.Count > 0) verbalization = verbalization + "Then, this evening, " + VerbalizeAllEvents(morniningEvents) + ". ";

        return verbalization;

    }
}
