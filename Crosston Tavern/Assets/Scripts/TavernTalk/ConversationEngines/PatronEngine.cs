using System.Collections;
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
                } else return new SocialMove("order#OffRecomendation", new List<string>() { favoriteFood.name });
            case "serveOrder#":
                FoodItem servedFood = GetFromMenu(prompt.arguements[0]);

                Dictionary<PreferenceLevel, List<string>> ingredientOpinions = GetOpinionIngredients(servedFood);
                PreferenceLevel opinionOfDish = speaker.townieInformation.ItemPreference(servedFood.name);



                if (opinionOfDish != PreferenceLevel.neutral)
                    facts.Add(new WorldFactPreference(speaker.Id, opinionOfDish, servedFood.name));
                foreach (PreferenceLevel level in ingredientOpinions.Keys) {
                    foreach (string ingredient in ingredientOpinions[level]) {
                        facts.Add(new WorldFactPreference(speaker.Id, level, ingredient));
                    }
                }


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
                facts = GoalsToFacts(GetStuckGoals(), speaker.Id);
                foreach(WorldFact stuckFact in facts) {
                    if(stuckFact is WorldFactGoal goalFact) {
                        goalFact.modifier.Add("stuck");
                    }
                }


                return new SocialMove("frustratedByGoals", mentionedFacts: facts);


            case "askWhyAction#":
                return SocialMoveFactory.MakeMove("tellWhyAction#", speaker, prompt);
            case "askAboutExcitement":
                return SocialMoveFactory.MakeMove("tellAboutEXCITEDEvent", speaker, prompt);
            case "askAboutDisapointment":
                return SocialMoveFactory.MakeMove("tellAboutDISAPOINTEDEvent", speaker, prompt);
            case "askAboutPreferencesLike":
                return SocialMoveFactory.MakeMove("tellPreferenceLike", speaker, prompt);
            case "askAboutPreferencesHate":
                return SocialMoveFactory.MakeMove("tellPreferenceHate", speaker, prompt);
            case "tellAction#":
            case "tellPreference#":
                return SocialMoveFactory.MakeMove("acknowledge", speaker, prompt);

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
                AskQuestion();
                
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


                if (prompt.mentionedFacts[0] is WorldFactPotentialAction potentialAction) {
                    BoundAction suggestedAction = potentialAction.action;

                    Goal goal = new GoalAction(suggestedAction, (int)GoalManager.GoalPriority.high);
                    speaker.gm.AddModule(new GoalModule(new List<GM_Precondition>() {
                                                            new GM_Precondition_PlayerInstructed("barkeep", speaker.Id)
                                                        },
                                                        new List<Goal>() {
                                                            goal
                                                        },
                                                        name: "suggested action",
                                                        timer: 3
                                                        )
                        );

                    WorldFactGoal goalFact = new WorldFactGoal(goal, speaker.Id);
                    goalFact.modifier.Add("player");

                    return new SocialMove("acceptSuggestion#",
                                        arguements: new List<string> { goal.ToString() },
                                        mentionedFacts: new List<WorldFact>() { goalFact });
                }

                throw new System.Exception("1st mentioned fact (" + prompt.mentionedFacts[0] + ") was not a potentialAction");

            case "cancelSuggestion#":
                return new SocialMove("acceptCancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);

            case "stopPlayerGivenGoal#":

                if(prompt.mentionedFacts[0] is WorldFactGoal gf) {
                    if (gf.modifier.Contains("player")) {
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
                SocialMove move = SocialMoveFactory.MakeMove("tellRelationWith#", speaker, prompt);
                move.verb = "acceptRelationshipView";

                return move;

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

                return new SocialMove("goodbye");
            case "giveRecipe#":
                TellAction("kitchen_" + speaker.homeLocation, foodItem.verb.verbPresent + "_" + foodItem.name);

                return new SocialMove("goodbyeThank");
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

    List<Goal> GetStuckGoals()
    {
        return speaker.gm.GetStuckGoals();
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
} 
