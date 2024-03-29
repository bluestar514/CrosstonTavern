﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatronEngine :ConversationEngine
{
    

    bool hasOrdered = false;
    FoodItem foodItem = null;
    
    int turns = 0;

    public int MaxTurns { get; private set; } = 5;


    public PatronEngine(Townie patron, List<FoodItem> barMenu)
    {
        speaker = patron;
        this.barMenu = barMenu;
    }

    public int DecrementTurns()
    {
        MaxTurns--;
        return MaxTurns;
    }

    void AskQuestion()
    {
        MaxTurns++;
    }


    public SocialMove GiveResponse(SocialMove prompt)
    {
        List<WorldFact> facts = new List<WorldFact>();

        switch (prompt.verb) {
            case "start":
                return new SocialMove("greet");
            case "greet":
            case "askForOrder":
                AskQuestion();

                float coin = Random.value;
                if (coin > .5) {
                    FoodItem food = GetFavoriteFoodFromMenu();
                    return new SocialMove("order#", new List<string>() { food.name });
                } else {
                    return new SocialMove("askForRecomendation");
                }
            case "recomend#":
                FoodItem recomendedFood = GetFromMenu(prompt.arguements[0]);
                FoodItem favoriteFood = GetFavoriteFoodFromMenu();

                int scoreOfRecomended = ScoreFood(recomendedFood);
                int scoreOfFavorite = ScoreFood(favoriteFood);

                if (scoreOfRecomended >= Mathf.Abs(scoreOfFavorite / 4)) {
                    return new SocialMove("order#OnRecomendation", new List<string>() { recomendedFood.name });
                } else {

                    KeyValuePair<PreferenceLevel, List<WorldFact>> opReason = GetOpinionOnDish(recomendedFood);
                    facts = opReason.Value;
                    PreferenceLevel opinion = opReason.Key;

                    return new SocialMove("order#OffRecomendation",
                                                new List<string>() { favoriteFood.name },
                                                mentionedFacts:facts);

                }
            case "serveOrder#":
                FoodItem servedFood = GetFromMenu(prompt.arguements[0]);

                KeyValuePair<PreferenceLevel, List<WorldFact>> opinionReason = GetOpinionOnDish(servedFood);
                facts = opinionReason.Value;
                PreferenceLevel opinionOfDish = opinionReason.Key;

                scoreOfRecomended = ScoreFood(servedFood);

                if (scoreOfRecomended < 0) MaxTurns = 3;
                else if (scoreOfRecomended < 3) MaxTurns = 5;
                else MaxTurns = 7;


                hasOrdered = true;
                foodItem = servedFood;

                return new SocialMove("thank", arguements: new List<string>() { opinionOfDish.ToString() }, mentionedFacts: facts);

            case "askAboutState":
                return SocialMoveFactory.MakeMove("tellState#", speaker, prompt);
            case "console":
            case "congratulate":
                if (!hasOrdered) goto case "greet";
                else return new SocialMove("thank");


            case "askAboutGoals":
                return SocialMoveFactory.MakeMove("tellAboutGoals", speaker, prompt);
            case "askAboutDayFull":
                return SocialMoveFactory.MakeMove("tellAboutDayEvents", speaker, prompt);
            case "askAboutDayHighlights":
                return SocialMoveFactory.MakeMove("tellAboutNOTEWORTHYEvent", speaker, prompt);
            case "askAboutObservation":
                return SocialMoveFactory.MakeMove("tellAboutDayObservedEvents", speaker, prompt);

            case "askAboutGoalFrustration":
                return SocialMoveFactory.MakeMove("tellFrustration", speaker, prompt);

            case "askWhyAction#":
                return SocialMoveFactory.MakeMove("tellWhyAction#", speaker, prompt);
            case "askAboutExcitement":
                return SocialMoveFactory.MakeMove("tellAboutEXCITEDEvent", speaker, prompt);
            case "askAboutDisapointment":
                return SocialMoveFactory.MakeMove("tellAboutDISAPOINTEDEvent", speaker, prompt);
            case "askAboutPlayerGivenGoal#":
                return SocialMoveFactory.MakeMove("tellAboutPlayerDirectedEvent", speaker, prompt);



            case "askAboutPreferencesLike":
                return SocialMoveFactory.MakeMove("tellPreferenceLike", speaker, prompt);
            case "askAboutPreferencesHate":
                return SocialMoveFactory.MakeMove("tellPreferenceHate", speaker, prompt);
            case "tellAction#":
                if (prompt.complexFacts[0] is WorldFactEvent e) {

                    if (speaker.ws.knownFacts.KnowFact(e)) {
                        return new SocialMove("alreadyKnew");
                    } else {
                        speaker.ws.LearnFact(e);

                        return new SocialMove("didNotKnow");
                    }
                }
                throw new System.Exception("Incorrect format");
            case "tellPreference#":
                if(prompt.complexFacts[0] is WorldFactPreference preference) {

                    if(speaker.ws.map.GetPerson(preference.person).preference.GetLevel(preference.item) == preference.level) {
                        return new SocialMove("alreadyKnew");
                    } else {
                        speaker.ws.knownFacts.AddFact(preference, speaker.ws);

                        return new SocialMove("didNotKnow");
                    }
                }
                throw new System.Exception("Incorrect format");
            case "confirmGoal#":
                WorldFact fact = prompt.mentionedFacts[0];

                if (fact is WorldFactGoal) {
                    Goal factGoal = ((WorldFactGoal)fact).goal;
                    Goal registeredGoal = speaker.gm.GetGoalFromName(factGoal.name);
                    if (registeredGoal == null) return new SocialMove("confirmGoal#OutOfDate",
                                                 new List<string> { factGoal.name },
                                                 retractedFacts: new List<WorldFact>() { fact });
                    else return new SocialMove("confirmGoal#InDate",
                                                new List<string>() { registeredGoal.name },
                                                mentionedFacts: new List<WorldFact>() { new WorldFactGoal(registeredGoal, speaker.Id) });
                }


                throw new System.Exception("Fact included in confirmGoal# SocialMove was not a WorldFactGoal or no fact was provided.");

            case "suggest#":
            case "suggest_#":
                AskQuestion();
                
                if(speaker.gm.GetPlayerSpecifiedGoals().Count >= 3) {
                    return new SocialMove("makeRoomForPlayerGoal#",
                                            arguements: prompt.arguements,
                                            mentionedFacts: prompt.mentionedFacts);
                }

                if(prompt.mentionedFacts[0] is WorldFactPotentialAction factAction &&
                    !speaker.KnowsAboutAction(factAction.action)) {
                    return new SocialMove("askHowToDo#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);
                } 

                return new SocialMove("askConfirmSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);

            

            case "tellHowToDo#":
                if(prompt.mentionedFacts[0] is WorldFactPotentialAction factPotentialAction) {
                    BoundAction boundAction = factPotentialAction.action;

                    TellAction(boundAction.FeatureId, boundAction.Id);
                }

                goto case "confirmSuggestion#";
            case "confirmSuggestion#":

                return AddGoal(prompt);


            case "cancelSuggestion#":
                return new SocialMove("acceptCancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);

            case "changeGoal#->#":
                if(prompt is CompoundSocialMove compoundPrompt) {
                    List<SocialMove> moves = new List<SocialMove>();
                    foreach(SocialMove submove in compoundPrompt.socialMoves) {
                        SocialMove newMove = GiveResponse(submove);
                        if(newMove.verb == "askConfirmSuggestion#") {
                            newMove = AddGoal(newMove);
                        }

                        moves.Add(newMove);
                    }

                    return new CompoundSocialMove("acceptChangeGoal#->#",
                                                    arguements: prompt.arguements,
                                                    socialMoves: moves);
                }
                throw new System.Exception("Incorrect input format for \"changeGoal#->#\" (" + prompt + ")");

            case "stopPlayerGivenGoal#":

                if(prompt.mentionedFacts[0] is WorldFactGoal gf) {
                    if (gf.modifier.Contains(WorldFactGoal.Modifier.player)) {
                        speaker.gm.GetParentModule(gf.goal).ForEach(module => {
                            speaker.gm.RemoveModule(module);
                        });
                    }
                }

                return new SocialMove("acceptStopGoal#", arguements: prompt.arguements, retractedFacts: prompt.mentionedFacts);

            case "askRelationWith":
                return new SocialMove("passOpenAskRelationWith");
            case "askRelationWith#":
                return SocialMoveFactory.MakeMove("tellRelationWith#", speaker, prompt);

            case "tell#Relation#":
                if (prompt.complexFacts.Count == 1) {
                    Dictionary<string, List<Relationship.Tag>> opinionOnNewInfo = new Dictionary<string, List<Relationship.Tag>>();

                    string source = "";
                    string target = "";
                    Relationship rel = new Relationship();

                    WorldFact complexFact = prompt.complexFacts[0];
                    if (complexFact is WorldFactRelation relationFact) {
                        StateRelation relation = relationFact.relation;

                        source = relation.source;
                        target = relation.target;

                        rel = speaker.ws.GetRelationshipsFor(source);
                        List<Relationship.Tag> knownTags = rel.GetTag(target);

                        if (knownTags.Contains(relation.tag)) {
                            AddOpinion("known", relation.tag, opinionOnNewInfo);
                        } else {
                            AddOpinion("unknown", relation.tag, opinionOnNewInfo);
                        }

                    }


                    AddOpinion("known",
                                rel.GetStrongestTagOnAxis(target, Relationship.Axis.friendly),
                                opinionOnNewInfo);
                    AddOpinion("known",
                                rel.GetStrongestTagOnAxis(target, Relationship.Axis.romantic),
                                opinionOnNewInfo);

                    LearnFromInput(prompt.complexFacts, new List<WorldFact>());

                    SocialMove move = SocialMoveFactory.MakeMove("tellRelationWith#", speaker, prompt);
                    move.verb = "acceptRelationshipView";


                    foreach (string catagory in new List<string>() { "known", "unknown" }) { 
                        if (opinionOnNewInfo.ContainsKey(catagory)) {

                            move.complexFacts.AddRange( 
                                new List<WorldFact>(from unknown in opinionOnNewInfo[catagory]
                                                    select new WorldFactRelation(new StateRelation(source, target, unknown), source)));
                        }
                    }

                    return move;

                }

                throw new System.Exception("Incorrect input to \"tell#Relation#\"");

                void AddOpinion(string opinion, Relationship.Tag tag, Dictionary<string, List<Relationship.Tag>> opinionOnNewInfo)
                {
                    if (!opinionOnNewInfo.ContainsKey(opinion)) {
                        opinionOnNewInfo.Add(opinion, new List<Relationship.Tag>());
                    }

                    opinionOnNewInfo[opinion].Add(tag);
                }

            case "askWhyGoal":
                Debug.LogWarning("This is an out of date pattern, consider removing");
                return new SocialMove("passOpenAskWhyGoal");
            case "askWhyGoal#":
                return SocialMoveFactory.MakeMove("tellWhyGoal#", speaker, prompt);

            case "turnsUp":
                MaxTurns = 100;

                List<Goal> goals = speaker.gm.GetGoalsList();

                if (goals != null &&
                    goals.Count > 0 &&
                    foodItem != null &&
                    goals.Any(goal => goal is GoalState goalState &&
                                   goalState.state is StateInventory invState &&
                                   invState.itemId == foodItem.name) &&
                    !HasRecipe(foodItem) ) {
                    return new SocialMove("askForRecipe#", arguements: new List<string>() { foodItem.name });
                }

                return new SocialMove("goodbyeThank");
            case "giveRecipe#":
                TellAction("kitchen_" + speaker.homeLocation, foodItem.verb.verbPresent + "_" + foodItem.name);

                return new SocialMove("goodbyeThankRecipe");
            case "refuseRecipe#":
                return new SocialMove("goodbyeDejected");

            case "goodbye":
                return new SocialMove("ENDCONVERSATION");


            case "nice":
                return new SocialMove("pass");
            default:
                return new SocialMove("DEFAULT");
        }
    }



    void TellAction(string featureName, string actionName )
    {
        Feature feature = speaker.ws.map.GetFeature(featureName);
        if (feature == null) throw new System.Exception("no feature with name " + featureName);

        GenericAction action = ActionInitializer.GetAllActions()[actionName];

        feature.providedActions.Add(action);
    }


    bool HasRecipe(FoodItem foodItem)
    {
        Feature feature = speaker.ws.map.GetFeature("kitchen_" + speaker.homeLocation);
        if (feature == null) throw new System.Exception("no feature with name: " + "kitchen_" + speaker.homeLocation);

        return feature.providedActions.Any(action => action.Id == foodItem.verb.verbPresent + "_" + foodItem.name);
    }


    KeyValuePair<PreferenceLevel, List<WorldFact>> GetOpinionOnDish(FoodItem servedFood)
    {
        Debug.Log(servedFood.name);

        List<WorldFact> facts = new List<WorldFact>();

        Dictionary<PreferenceLevel, List<string>> ingredientOpinions = GetOpinionIngredients(servedFood);
        PreferenceLevel opinionOfDish = speaker.townieInformation.ItemPreference(servedFood.name);

        if (opinionOfDish != PreferenceLevel.neutral) {
            facts.Add(new WorldFactPreference(speaker.Id, opinionOfDish, servedFood.name));
            Debug.Log(facts.Last());
        }

        foreach (PreferenceLevel level in ingredientOpinions.Keys) {
            foreach (string ingredient in ingredientOpinions[level]) {
                facts.Add(new WorldFactPreference(speaker.Id, level, ingredient));
                Debug.Log(facts.Last());
            }
        }

        return new KeyValuePair<PreferenceLevel, List<WorldFact>>(opinionOfDish, facts);
    }


    SocialMove AddGoal(SocialMove prompt)
    {
        if (prompt.mentionedFacts[0] is WorldFactPotentialAction potentialAction) {
            BoundAction suggestedAction = potentialAction.action;

            Goal goal = new GoalAction(suggestedAction, (int)GoalManager.GoalPriority.imperitive);
            speaker.gm.AddModule(new GoalModule(new List<GM_Precondition>() {
                                                            new GM_Precondition_PlayerInstructed("barkeep", speaker.Id)
                                                        },
                                                new List<Goal>() {
                                                            goal
                                                },
                                                name: "suggested action"
                                                //timer: 3
                                                )
                );

            WorldFactGoal goalFact = new WorldFactGoal(goal, speaker.Id);
            goalFact.modifier.Add(WorldFactGoal.Modifier.player);

            return new SocialMove("acceptSuggestion#",
                                arguements: new List<string> { goal.ToString() },
                                mentionedFacts: new List<WorldFact>() { goalFact });
        }

        throw new System.Exception("1st mentioned fact (" + prompt.mentionedFacts[0] + ") was not a potentialAction");

    }
} 
