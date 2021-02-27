﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarkeepEngine : ConversationEngine
{
    Person patron;
    string patronGeneralMood = "neutral";
    string patronId;


    public BarkeepEngine(Townie speaker, Person patron, List<FoodItem> barMenu)
    {
        this.speaker = speaker;

        this.patron = patron;
        patronId = patron.id;

        this.barMenu = barMenu;
    }

    public List<SocialMove> GetSocialMoves(SocialMove prompt)
    {
        List<SocialMove> moves = new List<SocialMove>();

        switch (prompt.verb) {
            case "greet":
                moves.Add(new SocialMove("askAboutState"));
                moves.Add(new SocialMove("greet"));
                moves.Add(new SocialMove("askForOrder"));
                break;

            case "askForRecomendation":
                foreach (FoodItem item in barMenu) {
                    moves.Add(new SocialMove("recomend#", new List<string>() { item.name }));
                }
                break;

            case "order#":
            case "order#OnRecomendation":
            case "order#OffRecomendation":
                moves.Add(new SocialMove("serveOrder#", prompt.arguements));
                break;

            case "tellState#":
                switch (prompt.arguements[0]) {
                    case "sad":
                    case "angry":
                        moves.Add(new SocialMove("console", prompt.arguements));
                        break;
                    case "happy":
                        moves.Add(new SocialMove("congratulate", prompt.arguements));
                        break;
                }

                patronGeneralMood = prompt.arguements[0];

                break;
            case "tellStateNONE":
                moves.Add(new SocialMove("congratulate"));
                break;

            case "tellAboutDayEvents":
            case "tellAbout#Event":
            case "tellWhyAction#":
            case "tellAboutGoals":
                if (prompt.mentionedFacts.Count > 0) {
                    moves.AddRange(GenAskWhyAction(prompt.mentionedFacts));
                    moves.AddRange(GenAskWhyGoal(prompt.mentionedFacts));
                }
                moves.Add(new SocialMove("nice"));
                break;


            case "tellWhyGoal#":
                List<BoundAction> suggestedActions = GenSuggestedAction(prompt.mentionedFacts[0]);

                if (suggestedActions.Count > 0) {
                    moves.Add(new SocialMove("suggest", arguements: prompt.arguements,
                        mentionedFacts: new List<WorldFact>(from action in suggestedActions
                                                            select new WorldFactPotentialAction(action))));
                }

                moves.Add(new SocialMove("nice"));
                break;
            case "passOpenSuggestions":

                moves.AddRange(from fact in prompt.mentionedFacts
                               select new SocialMove("suggest#", arguements: new List<string>() { fact.ToString() },
                               mentionedFacts: new List<WorldFact>() { fact }));
                moves.Add(new SocialMove("nevermind"));
                break;
            case "askConfirmSuggestion#":
                moves.Add(new SocialMove("confirmSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts));
                moves.Add(new SocialMove("cancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts));
                break;

            case "passOpenAskRelationWith":
                moves.AddRange(GenAskOpinionOfPerson());
                moves.Add(new SocialMove("nevermind"));
                break;

            case "tellRelationWith#":
                moves.AddRange(GenTellRelationshipGuess(prompt.arguements[0]));
                moves.Add(new SocialMove("nice"));
                break;

            case "askForRecipe#":
                moves.Add(new SocialMove("giveRecipe#", arguements: prompt.arguements));
                moves.Add(new SocialMove("refuseRecipe#", arguements: prompt.arguements));
                break;
            case "goodbye":
            case "goodbyeThank":
            case "goodbyeDejected":
                moves.Add(new SocialMove("goodbye"));
                break;
            default:
                List<SocialMove> barkeeperMoves = new List<SocialMove>() {
                    new SocialMove("askAboutGoals"),
                    //new SocialMove("askAboutGoalFrustration"),
                    //new SocialMove("askAboutDayFull"),
                    new SocialMove("askAboutDayHighlights", arguements: new List<string>(){ patronGeneralMood }),
                    //new SocialMove("askAboutObservation"),
                    //new SocialMove("askAboutExcitement"),
                    //new SocialMove("askAboutDisapointment"),
                    //new SocialMove("askWhyGoal"),
                    new SocialMove("askAboutPreferencesLike"),
                    new SocialMove("askAboutPreferencesHate")
                };

                moves = new List<SocialMove>(barkeeperMoves);

                moves.Add(new SocialMove("askRelationWith"));

                List<WorldFact> facts = speaker.ws.knownFacts.GetFacts();

                facts = TrimOldEvents(facts);
                facts = TrimSimilarFacts(facts);

                moves.AddRange(GenAskWhyGoal(facts));
                moves.AddRange(GenAskWhyAction(facts));
                moves.AddRange(GenTellAction());
                moves.AddRange(GenTellPreference());
                moves.AddRange(GenConfirmGoal(facts));
                //moves.AddRange(GenSuggestedAction());
                //moves.AddRange(GenAskAboutAction());
                break;
        }


        return RemoveAlreadyAskedQuestions(moves);

    }

    List<SocialMove> GenAskWhyGoal(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == patronId
                                    select new SocialMove("askWhyGoal#", new List<string> { ((WorldFactGoal)fact).goal.name },
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenAskWhyAction(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == patronId //I may take out this condition in the long run
                                    select new SocialMove("askWhyAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() },
                                                                           mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenTellAction()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId != patronId
                                    where ((WorldFactEvent)fact).action.Action.FeatureId != patronId
                                    select new SocialMove("tellAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenAskAboutAction()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == patronId
                                    select new SocialMove("askAboutAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenTellPreference()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactPreference
                                    where ((WorldFactPreference)fact).person != patronId
                                    select new SocialMove("tellPreference#", new List<string> { fact.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenConfirmGoal(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == patronId
                                    select new SocialMove("confirmGoal#", new List<string> { ((WorldFactGoal)fact).goal.name },
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<BoundAction> GenSuggestedAction(WorldFact fact)
    {
        ActionBuilder ab = new ActionBuilder(speaker.ws, patron);

        List<BoundAction> possibleActions = ab.GetAllActions(respectLocation: false);

        System.Func<BoundAction, bool> filter = action => true;

        if (fact is WorldFactGoal factGoal) {
            Goal goal = factGoal.goal;

            if (goal is GoalAction) return new List<BoundAction>() { };

            GoalState goalState = (GoalState)goal;

            State state = goalState.state;

            if (state is StateInventory stateInv) {
                filter = action => {
                    return (speaker.ws.map.GetFeature(action.FeatureId).type != Feature.FeatureType.person &&
                            speaker.ws.map.GetFeature(action.FeatureId).type != Feature.FeatureType.door) ||
                            (action.Id.StartsWith("ask_") && action.Id.Contains(stateInv.itemId));
                };
            }
            if (state is StateSocial stateSocial) {
                filter = action => {
                    return
                            action.FeatureId == stateSocial.targetId;
                };
            }
            if (state is StateRelation stateRelation) {
                filter = action => {
                    return
                            action.FeatureId == stateRelation.target;
                };
            }
        }

        return new List<BoundAction>(from action in possibleActions
                                     where filter(action)
                                     select action
                                    );
    }

    List<SocialMove> GenAskOpinionOfPerson(List<string> people = null)
    {
        if (people == null) people = speaker.townieInformation.relationships.GetKnownPeople();

        return new List<SocialMove>(from person in people
                                    where person != speaker.Id && person != "barkeep" && person != patronId
                                    select new SocialMove("askRelationWith#", new List<string>() { person }));
    }

    List<SocialMove> GenTellRelationshipGuess(string target)
    {
        return new List<SocialMove>(from relation in Relationship.codifiedRelationRanges.Keys
                                    select new SocialMove("tell#Relation#",
                                                            arguements: new List<string>() { target, relation.ToString() },
                                                            mentionedFacts: new List<WorldFact>() {
                                                                new WorldFactRelation(new StateRelation(target, patronId, relation))
                                                            })
                                    );
    }



    List<SocialMove> RemoveAlreadyAskedQuestions(List<SocialMove> moves)
    {
        List<string> repeatableActions = new List<string>() { "console", "congratulate", "acknowledge", "nice", "suggest", "nevermind", "askRelationWith" };

        List<SocialMove> finalList = new List<SocialMove>();

        foreach (SocialMove move in moves) {
            if (repeatableActions.Contains(move.verb)) {
                finalList.Add(move);
                continue;
            }

            bool alreadyAsked = false;
            foreach (SocialMove previous in executedMoves) {
                if (move.ToString() == previous.ToString()) {
                    alreadyAsked = true;
                }

                //if(move.verb == previous.verb) {
                //    if(move.mentionedFacts.Count > 0 && FactsMatch(move.mentionedFacts, previous.mentionedFacts)) {
                //        continue;
                //    } else {
                //        alreadyAsked = true;
                //        break;
                //    }
                //} 
            }

            if (!alreadyAsked) finalList.Add(move);
        }

        return finalList;
    }

    List<WorldFact> TrimOldEvents(List<WorldFact> facts)
    {
        return new List<WorldFact>((from fact in facts
                                    where fact is WorldFactEvent
                                    where GetDayDif((WorldFactEvent)fact) < 7
                                    select fact)
                                   .Concat(
                                    from fact in facts
                                    where !(fact is WorldFactEvent)
                                    select fact));
    }

    int GetDayDif(WorldFactEvent fact)
    {
        int dif = speaker.ws.Time.ConvertToDayCount() - fact.action.executionTime.ConvertToDayCount();
        return dif;
    }

    List<WorldFact> TrimSimilarFacts(List<WorldFact> facts)
    {
        List<WorldFact> finalList = new List<WorldFact>(from fact in facts
                                                        where !(fact is WorldFactEvent)
                                                        select fact);
        List<WorldFactEvent> eventFacts = new List<WorldFactEvent>(from fact in facts
                                                                   where (fact is WorldFactEvent)
                                                                   orderby ((WorldFactEvent)fact).action.executionTime.ConvertToDayCount()
                                                                   select (WorldFactEvent)fact);
        eventFacts.Reverse();

        Dictionary<VerbActorFeature, int> actionTimesDict = new Dictionary<VerbActorFeature, int>();

        foreach (WorldFactEvent fact in eventFacts) {

            VerbActorFeature actionSummary = new VerbActorFeature(fact.action.Action.Id,
                                                                  fact.action.Action.ActorId,
                                                                  fact.action.Action.FeatureId);
            if (!actionTimesDict.ContainsKey(actionSummary)) {
                actionTimesDict.Add(actionSummary, 0);

            }
            if (actionTimesDict.ContainsKey(actionSummary)
                && actionTimesDict[actionSummary] < 3) {
                finalList.Add(fact);
                actionTimesDict[actionSummary]++;
            }

        }

        return finalList;
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
