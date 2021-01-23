using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SocialMoveFactory
{
    public static SocialMove MakeTellAboutGoals(Townie speaker)
    {
        List<WorldFact> facts = MakeGoalsFacts(speaker, speaker.gm.GetGoalsList());

        return new SocialMove("tellAboutGoals", mentionedFacts: facts);
    }

    public static SocialMove MakeTellAboutDayEvents(Townie speaker)
    {
        List<ExecutedAction> history = GetDayEvents(speaker);
        history = FilterMyActions(speaker, history);

        List<WorldFact> facts = MakeActionFacts(history);

        return new SocialMove("tellAboutDayEvents", mentionedFacts: facts);
    }

    public static SocialMove MakeTellAboutInterestingEvents(Townie speaker, Opinion.Tag tag)
    {
        List<ExecutedAction> history = FilterMyActions(speaker, GetDayEvents(speaker));
        string arg = tag.ToString().ToUpper();
        switch (tag) {
            case Opinion.Tag.disapointed:
            case Opinion.Tag.excited:
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Contains(Opinion.Tag.excited)
                                                   select e);
                break;
            case Opinion.Tag.noteworthy:
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Count > 0
                                                   select e);
                break;
        }
        history = CondenseEvents(history);

        List<WorldFact> facts = MakeActionFacts(history);

        return new SocialMove("tellAbout#Event", arguements: new List<string>() { arg }, mentionedFacts: facts);
    }

    public static SocialMove MakeTellAboutObservedEvents(Townie speaker)
    {
        List<ExecutedAction> history = FilterOtherActions(speaker, GetDayEvents(speaker));
        List<WorldFact> facts = MakeActionFacts(history);

        return new SocialMove("tellAboutDayObservedEvents", mentionedFacts: facts);
    }

    public static SocialMove MakeTellWhyGoal(Townie speaker, SocialMove prompt)
    {
        string goalName = prompt.arguements[0];
        WorldFactGoal goalFact = (WorldFactGoal)prompt.mentionedFacts[0];
        Goal goal = goalFact.goal;
        List<WorldFact> facts;
        if (goal.unlockedActionsOnGoalCompletion.Count > 0) {
            int rand = Random.Range(0, goal.unlockedActionsOnGoalCompletion.Count);

            BoundAction potentialAction = goal.unlockedActionsOnGoalCompletion[rand];

            WorldFact fact = new WorldFactPotentialAction(potentialAction);
            facts = new List<WorldFact>() { fact };
        } else {
            facts = new List<WorldFact>();
        }
        List<Goal> parentGoals = goal.GetParentGoals();


        facts.AddRange(MakeGoalsFacts(speaker, parentGoals));

        return new SocialMove("tellWhyGoal#", new List<string>() { goalName }, mentionedFacts: facts);
    }

    public static SocialMove MakeTellWhyAction(Townie speaker, SocialMove prompt)
    {
        string actionName = prompt.arguements[0];
        ExecutedAction action = speaker.ws.knownFacts.GetActionFromName(actionName);
        if (action == null) return new SocialMove("dontKnow#", prompt.arguements);

        List<Goal> goals = new List<Goal>(from rational in action.Action.weightRationals
                                          select rational.goal);
        List<WorldFact> facts = MakeGoalsFacts(speaker, goals);

        return new SocialMove("tellWhyAction#", new List<string>() { actionName },
                                        mentionedFacts:facts);
    }

    public static SocialMove MakeTellPreference(Townie speaker, bool liked)
    {
        if (liked) return MakeTellPreferenceHelper(speaker, PreferenceLevel.loved, PreferenceLevel.liked);
        else return MakeTellPreferenceHelper(speaker, PreferenceLevel.hated, PreferenceLevel.disliked);
    }
    static SocialMove MakeTellPreferenceHelper(Townie speaker, 
            PreferenceLevel level1, PreferenceLevel level2)
    {
        List<WorldFact> facts = new List<WorldFact>();
        List<string> items1 = speaker.townieInformation.preference.preferences[level1];
        List<string> items2 = speaker.townieInformation.preference.preferences[level2];

        int total = items1.Count + items2.Count;
        int rand = Random.Range(0, total);

        string item;
        PreferenceLevel level;
        if (rand < items1.Count) {
            item = items1[rand];
            level = level1;
        } else {
            item = items2[rand - items1.Count];
            level = level2;
        }

        facts.Add(new WorldFactPreference(speaker.townieInformation.id, level, item));

        return new SocialMove("tellPreference", arguements: new List<string> { level.ToString() },
                                                mentionedFacts: facts);
    }


    // Helpers:
    static List<ExecutedAction> GetDayEvents(Townie speaker)
    {
        List<ExecutedAction> completeHistory = speaker.ws.knownFacts.GetHistory();
        List<ExecutedAction> todaysHistory = completeHistory.FindAll(x => x.executionTime.SameDay(speaker.ws.Time));

        return todaysHistory;
    }
    static List<ExecutedAction> FilterMyActions(Townie speaker, List<ExecutedAction> todaysHistory)
    {
        List<ExecutedAction> filteredHistory = todaysHistory.FindAll(x => x.Action.ActorId == speaker.townieInformation.id ||
                                                                            x.Action.FeatureId == speaker.townieInformation.id);

        return filteredHistory;
    }
    static List<ExecutedAction> FilterOtherActions(Townie speaker, List<ExecutedAction> observedActions)
    {
        List<ExecutedAction> filteredHistory = observedActions.FindAll(x => x.Action.ActorId != speaker.townieInformation.id &&
                                                                            x.Action.FeatureId != speaker.townieInformation.id);

        return filteredHistory;
    }

    static List<WorldFact> MakeGoalsFacts(Townie speaker, List<Goal> goals)
    {
        List<WorldFact> facts = new List<WorldFact>();
        string owner = speaker.townieInformation.id;
        foreach (Goal goal in goals) {
            facts.Add(new WorldFactGoal(goal, owner));
        }

        return facts;
    }
    static List<WorldFact> MakeActionFacts(List<ExecutedAction> actions)
    {
        List<WorldFact> facts = new List<WorldFact>();
        foreach (ExecutedAction action in actions) {
            facts.Add(new WorldFactEvent(action));
        }

        return facts;
    }


    static List<ExecutedAction> CondenseEvents(List<ExecutedAction> fullList)
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
    static ExecutedAction CondenseTwoEvents(ExecutedAction a1, ExecutedAction a2)
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
    static List<Effect> CondenseEffects(List<Effect> fullList)
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
