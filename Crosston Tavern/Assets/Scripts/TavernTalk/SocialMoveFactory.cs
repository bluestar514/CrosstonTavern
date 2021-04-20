using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SocialMoveFactory
{
    static public SocialMove MakeMove(string verb, Townie speaker, SocialMove prompt)
    {
        switch (verb) {
            case "greet":
            case "askAboutGoals":
            case "askAboutDayFull":
            case "askAboutDayHighlights":
            case "askAboutObservation":
            case "askAboutExcitement":
            case "askAboutDisapointment":
            case "askAboutPreferencesLike":
            case "askAboutPreferencesHate":
                return new SocialMove(verb);



            case "tellAction#":
            case "tellAboutGoals":
                return MakeTellAboutGoals(speaker);
            case "tellAboutDayEvents":
                return MakeTellAboutDayEvents(speaker);
            case "tellAboutNOTEWORTHYEvent":
                return MakeTellAboutInterestingEvents(speaker, Opinion.Tag.noteworthy);
            case "tellAboutEXCITEDEvent":
                return MakeTellAboutInterestingEvents(speaker, Opinion.Tag.excited);
            case "tellAboutDISAPOINTEDEvent":
                return MakeTellAboutInterestingEvents(speaker, Opinion.Tag.disapointed);

            case "tellAboutPlayerDirectedEvent":
                List<SocialMove> moves = new List<SocialMove>();

                
                WorldFact playerDirectedGoalFact = prompt.mentionedFacts[0];
                if (playerDirectedGoalFact is WorldFactGoal factGoal) {
                    Goal goal = factGoal.goal;

                    //Get actions already taken to achieve goal
                    moves.Add(ReportActionsTowardGoal(speaker, goal));

                    //Get subgoals that still need to be completed and mark the ones they are stuck on
                    List<Goal> childGoals = speaker.gm.GetChildGoals(goal);
                    List<Goal> stuckGoals = speaker.gm.GetStuckGoals();

                    List<WorldFact> facts = new List<WorldFact>();
                    foreach(Goal child in childGoals) {
                        if (child is GoalState goalState &&
                            goalState.state.InEffect(speaker.ws, new BoundBindingCollection(), new FeatureResources()))
                            continue;

                        WorldFactGoal fact = new WorldFactGoal(child, speaker.Id);

                        if (stuckGoals.Contains(child)) fact.modifier.Add(WorldFactGoal.Modifier.stuck);

                        facts.Add(fact);
                    }

                    moves.Add(new SocialMove("tellToDo", mentionedFacts: facts));

                }
                return new CompoundSocialMove("tellAboutPlayerDirectedEvent", socialMoves: moves);
            case "tellAboutDayObservedEvents":
                return MakeTellAboutObservedEvents(speaker);
            case "tellWhyGoal#":
                WorldFactGoal goalFact = (WorldFactGoal)prompt.mentionedFacts[0];

                return MakeTellWhyGoal(speaker, goalFact);
            case "tellWhyAction#":
                return MakeTellWhyAction(speaker, prompt);
            case "tellPreferenceLike":
                return MakeTellPreference(speaker, true);
            case "tellPreferenceHate":
                return MakeTellPreference(speaker, false);
            case "tellState#":
                return MakeTellState(speaker);
            case "tellRelationWith#":
                string subject = prompt.arguements[0];

                return MakeTellRelation(speaker, subject);

            case "tellFrustration":
                return MakeTellFrustrations(speaker);


            case "acknowledge":
                return new SocialMove("acknowledge", mentionedFacts: prompt.mentionedFacts);



            case "askWhyGoal#":
            case "askWhyAction#":
            case "askAboutAction#":
                throw new System.Exception(verb + 
                    " is currently only created enmass from the ConversationEngine, not from this Factory");
            default:
                return new SocialMove("DEFAULT");
        }
    }


    public static SocialMove MakeTellAboutGoals(Townie speaker)
    {
        List<Goal> topGoals = speaker.gm.GetTopLevelGoals();


        List<SocialMove> moves = new List<SocialMove>();
        foreach(Goal goal in topGoals) {
            List<Goal> subGoals = speaker.gm.GetChildGoals(goal);
            subGoals =new List<Goal>( from g in subGoals
                                 where !(g is GoalState state &&
                                        state.state is StatePosition)
                                 select g);

            subGoals.Insert(0, goal);

            moves.Add(new SocialMove("tellAboutGoal#",
                                        arguements: new List<string>() { goal.ToString() },
                                        mentionedFacts: MakeGoalsFacts(speaker, subGoals)));
        }

        

        return new CompoundSocialMove("tellAboutGoals", socialMoves: moves);
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
        List<ExecutedAction> history = GetDayEvents(speaker);
        string arg = tag.ToString().ToUpper();
        switch (tag) {
            case Opinion.Tag.disapointed:
            case Opinion.Tag.excited:
            case Opinion.Tag.directed:
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Contains(tag)
                                                   select e);
                break;
            case Opinion.Tag.noteworthy:
                history = new List<ExecutedAction>(from e in history
                                                   where e.opinion.tags.Count > 1 ||
                                                        (e.opinion.tags.Count > 0 && !e.opinion.tags.Contains(Opinion.Tag.directed))
                                                   select e);
                //history.AddRange(GetMostCommonEvents(GetDayEvents(speaker), speaker.Id));
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



    /// <summary>
    /// TODO: Mark whether or not the goal is one the character still has
    /// </summary>
    /// <param name="speaker"></param>
    /// <param name="goalFact">Uses the goalFact from the conversation so 
    /// it is possible to veiw the rational at the time the goal was recorded</param>
    /// <returns> mentionedFacts: 0 - goal we are talking about, 
    ///                           1 - an action unlocked by completing this goal, 
    ///                           2+  reasons for goal
    /// if a top level goal: 0 - goal we are talking about
    ///                      1+ preconditions for parent module 
    /// </returns>
    public static SocialMove MakeTellWhyGoal(Townie speaker, WorldFactGoal goalFact)
    {
        Goal goal = goalFact.goal;
        
        List<WorldFact> facts = new List<WorldFact>() { goalFact };
        List<WorldFact> retracted = new List<WorldFact>();

        List<Goal> parentGoals = goal.GetParentGoals();

        if (parentGoals.Count != 0) {
            BoundAction potentialAction = null;
            if (goal.unlockedActionsOnGoalCompletion.Count > 0) {
                int rand = Random.Range(0, goal.unlockedActionsOnGoalCompletion.Count);

                potentialAction = goal.unlockedActionsOnGoalCompletion[rand];

                WorldFact fact = new WorldFactPotentialAction(potentialAction);
                facts.Add(fact);
            }

            Goal duplicateAction = parentGoals.Find(parentGoal => {
                return (parentGoal is GoalAction actionGoal) && actionGoal.action.Equals(potentialAction);
            });
            if (duplicateAction != null) {
                parentGoals.Remove(duplicateAction);
            }

            facts.AddRange(MakeGoalsFacts(speaker, parentGoals));

            return new SocialMove("tellWhyGoal#", new List<string>() { goal.name }, mentionedFacts: facts);
        } else {
            List<GoalModule> modules = speaker.gm.GetParentModule(goal);

            foreach(GoalModule module in modules) {
                foreach(GM_Precondition precondition in module.preconditions) {
                    facts.Add(new WorldFactGoalModulePrecondition(precondition));


                    if (precondition is GM_Precondition_State preState) {
                        WorldFact fact;

                        if (preState.state is StateRelation rel) {
                            fact = new WorldFactRelation(rel, speaker.Id);
                        } else {
                            fact = new WorldFactState(preState.state);
                        }

                        if (preState.want)
                            facts.Add(fact);
                        else
                            retracted.Add(fact);
                    }
                }
            }

            return new SocialMove("tellWhyGoal#TopLevel", 
                                    new List<string> { goal.name }, 
                                    mentionedFacts: facts, 
                                    retractedFacts: retracted);
        }
    }

    //TODO: make this not reliant on a "prompt" but rather the action in question
    public static SocialMove MakeTellWhyAction(Townie speaker, SocialMove prompt)
    {
        string actionName = prompt.arguements[0];
        ExecutedAction action = speaker.ws.knownFacts.GetActionFromName(actionName);
        if (action == null) return new SocialMove("dontKnow#", prompt.arguements);

        List<Goal> goals = new List<Goal>(from rational in action.Action.weightRationals
                                          select rational.goal);
        goals = GoalManager.CondenseGoals(goals);

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

    public static SocialMove MakeTellState(Townie speaker)
    {
        EntityStatusEffectType strongestState = speaker.townieInformation.statusEffectTable.GetStrongestStatus();

        if (strongestState != EntityStatusEffectType.special) {
            List<ExecutedAction> history = GetDayEvents(speaker);

            List<ExecutedAction> causes = new List<ExecutedAction>();

            foreach(ExecutedAction action in history) {
                List<Effect> effects = action.executedEffect;
                IEnumerable<EffectStatusEffect> effectStatusEffects = from effect in effects
                                                                where effect is EffectStatusEffect
                                                                select (EffectStatusEffect)effect;
                List<EntityStatusEffect> statusEffects = new List<EntityStatusEffect>( 
                                                                from effect in effectStatusEffects
                                                                where effect.status.type == strongestState
                                                                select effect.status);
                if(statusEffects.Count > 0) {
                    causes.Add(action);
                } 

            }

            causes = CondenseEvents(causes);

            return new SocialMove("tellState#", 
                new List<string>() { strongestState.ToString() }, 
                mentionedFacts: MakeActionFacts(causes));

        }
        //check and see if they are having difficulty with goals

        //check and see if they got to do things they like (TODO, no support currently)


        return new SocialMove("tellStateNONE");
    }

    static SocialMove MakeTellRelation(Townie speaker, string target)
    {
        List<WorldFact> facts = GetInterRelationship(speaker.townieInformation, target, speaker.Id);
        facts.AddRange(GetInterRelationship(speaker.ws.map.GetPerson(target), speaker.Id, speaker.Id));

        return new SocialMove("tellRelationWith#", arguements: new List<string>() { target }, mentionedFacts: facts);
    }

    //started to revise so it also told you what the high priority are, but that isn't actually used yet
    static SocialMove MakeTellFrustrations(Townie speaker)
    {
        List<Goal> highPriority = GetHighPriorityGoals(speaker);
        List<Goal> stuckGoals = GetStuckGoals(speaker);

        HashSet<Goal> allGoals = new HashSet<Goal>(stuckGoals);
        List<Goal> goalList = allGoals.Union(highPriority).ToList();

        List<WorldFact> facts = GoalsToFacts(stuckGoals, speaker.Id);

        foreach (WorldFact generalFact in facts) {
            if (generalFact is WorldFactGoal goalFact) {
                if (stuckGoals.Contains(goalFact.goal)) {
                    goalFact.modifier.Add(WorldFactGoal.Modifier.stuck);
                }
                if (highPriority.Contains(goalFact.goal)) {
                    goalFact.modifier.Add(WorldFactGoal.Modifier.highPriority);
                }
            }

        }


        return new SocialMove("frustratedByGoals", mentionedFacts: facts);
    }

    static SocialMove ReportActionsTowardGoal(Townie speaker, Goal goal)
    {
        List<ExecutedAction> events = GetDayEvents(speaker);//MakeTellAboutInterestingEvents(speaker, Opinion.Tag.directed);
        List<ExecutedAction> relevanthistory = new List<ExecutedAction>();

        

        List<string> args = new List<string>();
        
            

        foreach (ExecutedAction executedAction in events) {
            List<WeightedAction.WeightRational> rationals = executedAction.Action.weightRationals;

            if (rationals.Any(item => { return speaker.gm.IsDerivedFromGoal(item.goal, goal); })) {
                relevanthistory.Add(executedAction);
            }
            if (rationals.Any(item => { return item.goal.Equals(goal); })) {
                args.Add("completed");
            }
        }
        
        relevanthistory = CondenseEvents(relevanthistory);

        List<WorldFact> relevantFacts = new List<WorldFact>(from history in relevanthistory
                                                            select new WorldFactEvent(history));

        return new SocialMove("listActionsTaken", args, mentionedFacts: relevantFacts);
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

    static List<ExecutedAction> GetMostCommonEvents(List<ExecutedAction> fullList, string speakerId)
    {
        int count = 0;
        int initialCount = fullList.Count;

        Dictionary<VerbActorFeature, int> verbCounts = new Dictionary<VerbActorFeature, int>();
        Dictionary<VerbActorFeature, ExecutedAction> actions = new Dictionary<VerbActorFeature, ExecutedAction>();
        HashSet<string> actors = new HashSet<string>();

        
        while (fullList.Count > 0) {
            ExecutedAction action = fullList[0];
            
            fullList.RemoveAt(0);
            count++;

            if (action.Action.ActorId == speakerId) continue;

            VerbActorFeature verb = new VerbActorFeature(action.Action.Id, action.Action.ActorId, action.Action.FeatureId);
            verbCounts.Add(verb, 1);
            actions.Add(verb, action);
            actors.Add(verb.actor);

            for (int i = fullList.Count - 1; i >= 0; i--) {
                ExecutedAction combination = CondenseTwoEvents(action, fullList[i]);
                if (combination != null) {
                    action = combination;
                    fullList.RemoveAt(i);
                    count++;

                    verbCounts[verb]++;
                    actions[verb] = combination;
                }
            }

            
        }

        List<ExecutedAction> commonAction = new List<ExecutedAction>();

        foreach(string actor in actors) {
            IEnumerable<KeyValuePair<VerbActorFeature, int>> relevant = from verb in verbCounts
                                                                 where verb.Key.actor == actor || 
                                                                        verb.Key.feature == actor
                                                                 select verb;

            int max = 0;
            VerbActorFeature maxVerb = null; 
            foreach(KeyValuePair<VerbActorFeature, int> pair in relevant) {
                if (pair.Value >= max) maxVerb = pair.Key;
            }

            commonAction.Add(actions[maxVerb]);
        }

        return commonAction;
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


    static List<WorldFact> GetInterRelationship(Person subject, string target, string owner)
    {
        List<WorldFact> facts = new List<WorldFact>();

        foreach (Relationship.Axis axis in
                        new List<Relationship.Axis>() {
                            Relationship.Axis.friendly,
                            Relationship.Axis.romantic}) {
            int value = subject.relationships.Get(target, axis);
            facts.Add(new WorldFactState(new StateSocial(subject.id, target, axis, value, value)));
        }

        facts.AddRange(from tag in subject.relationships.GetTag(target)
                       select new WorldFactRelation(new StateRelation(subject.id, target, tag), owner));

        return facts;
    }

    static List<Goal> GetStuckGoals(Townie speaker)
    {
        return speaker.gm.GetStuckGoals();
    }

    static List<Goal> GetHighPriorityGoals(Townie speaker)
    {
        return speaker.gm.GetHighPriorityGoals(.1f);
    }

    static List<WorldFact> GoalsToFacts(List<Goal> goals, string owner)
    {
        return new List<WorldFact>(from goal in goals
                                   select new WorldFactGoal(goal, owner));
    }
    class VerbActorFeature
    {
        public string verb;
        public string actor;
        public string feature;

        public VerbActorFeature(string verb, string actor, string feature)
        {
            this.verb = verb;
            this.actor = actor;
            this.feature = feature;
        }

        public override string ToString()
        {
            return "<" + string.Join(",", new List<string>() { verb, actor, feature }) + ">";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VerbActorFeature)) return false;
            VerbActorFeature other = (VerbActorFeature)obj;

            return verb == other.verb &&
                actor == other.actor &&
                feature == other.feature;
        }

        public override int GetHashCode()
        {
            var hashCode = 1587983527;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(verb);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(actor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(feature);
            return hashCode;
        }
    }
}
