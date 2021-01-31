using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationEngine
{
    public Townie speaker;
    List<SocialMove> socialMoves;
    string partner;

    List<FoodItem> barMenu;

    bool hasOrdered = false;
    List<SocialMove> executedMoves = new List<SocialMove>();


    public ConversationEngine(Townie patron, string partner, List<FoodItem> barMenu)
    {
        speaker = patron;
        this.partner = partner;
        this.barMenu = barMenu;
    }


    public SocialMove GiveResponse(SocialMove prompt)
    {
        switch (prompt.verb) {
            case "start":
                return new SocialMove("greet");
            case "greet":
            case "askForOrder":
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

                if( scoreOfRecomended >= Mathf.Abs(scoreOfFavorite / 2)) {
                    return new SocialMove("order#OnRecomendation", new List<string>() { recomendedFood.name });
                }else return new SocialMove("order#OffRecomendation", new List<string>() { favoriteFood.name });
            case "serveOrder#":
                FoodItem servedFood = GetFromMenu(prompt.arguements[0]);

                Dictionary<PreferenceLevel, List<string>> ingredientOpinions = GetOpinionIngredients(servedFood);
                PreferenceLevel opinionOfDish = speaker.townieInformation.ItemPreference(servedFood.name);

                List<WorldFact> facts = new List<WorldFact>();

                if(opinionOfDish != PreferenceLevel.neutral) 
                    facts.Add(new WorldFactPreference(speaker.Id, opinionOfDish, servedFood.name));
                foreach(PreferenceLevel level in ingredientOpinions.Keys) {
                    foreach(string ingredient in ingredientOpinions[level]) {
                        facts.Add(new WorldFactPreference(speaker.Id, level, ingredient));
                    }
                }

                hasOrdered = true;

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
            case "askWhyGoal#":
                return SocialMoveFactory.MakeMove("tellWhyGoal#", speaker, prompt);
            case "askWhyAction#":
                return SocialMoveFactory.MakeMove("tellWhyAction#", speaker, prompt);
            case "askAboutExcitement":
                return SocialMoveFactory.MakeMove("tellAboutEXCITEDEvent", speaker, prompt);
            case "askAboutDisapointment":
                return SocialMoveFactory.MakeMove("tellAboutDISAPOINTEDEvent", speaker, prompt);
            case "askAboutAction#": //Currently not used, consider removing
                return new SocialMove("tellDetailsOfAction#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);
            case "askAboutPreferencesLike":
                return SocialMoveFactory.MakeMove("tellPreferenceLike", speaker, prompt);
            case "askAboutPreferencesHate":
                return SocialMoveFactory.MakeMove("tellPreferenceHate", speaker, prompt);
            case "tellAction#":
            case "tellPreference#":
                return SocialMoveFactory.MakeMove("acknowledge", speaker, prompt);

            case "confirmGoal#":
                WorldFact fact = prompt.mentionedFacts[0];

                if(fact is WorldFactGoal) {
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
            case "nice":
                return new SocialMove("pass");
            default:
                return new SocialMove("DEFAULT");
        }
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
                foreach(FoodItem item in barMenu) {
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

                break;
            case "tellStateNONE":
                moves.Add(new SocialMove("congratulate"));
                break;

            case "tellAboutDayEvents":
            case "tellAbout#Event":
            case "tellAboutGoals":
                if (prompt.mentionedFacts.Count > 0) {
                    moves.AddRange(GenAskWhyAction(prompt.mentionedFacts));
                    moves.AddRange(GenAskWhyGoal(prompt.mentionedFacts));
                }
                moves.Add(new SocialMove("nice"));
                break;
            default:
                List<SocialMove> barkeeperMoves = new List<SocialMove>() {
                    new SocialMove("askAboutGoals"),
                    //new SocialMove("askAboutGoalFrustration"),
                    //new SocialMove("askAboutDayFull"),
                    new SocialMove("askAboutDayHighlights"),
                    //new SocialMove("askAboutObservation"),
                    new SocialMove("askAboutExcitement"),
                    new SocialMove("askAboutDisapointment"),
                    new SocialMove("askAboutPreferencesLike"),
                    new SocialMove("askAboutPreferencesHate")
                };

                moves = new List<SocialMove>(barkeeperMoves);
                moves.AddRange(GenAskWhyGoal(speaker.ws.knownFacts.GetFacts()));
                moves.AddRange(GenAskWhyAction(speaker.ws.knownFacts.GetFacts()));
                moves.AddRange(GenTellAction());
                moves.AddRange(GenTellPreference());
                moves.AddRange(GenConfirmGoal(speaker.ws.knownFacts.GetFacts()));
                //moves.AddRange(GenAskAboutAction());
                break;
        }


        return RemoveAlreadyAskedQuestions( moves );
        
    }

    public void DoMove(SocialMove move)
    {
        executedMoves.Add(move);
    }

    public List<WorldFact> LearnFromInput(List<WorldFact> facts)
    {
        List<WorldFact> learnedFacts = new List<WorldFact>();

        foreach(WorldFact fact in facts) {
            learnedFacts.AddRange(speaker.ws.LearnFact(fact));
        }

        return learnedFacts;
    }


    List<SocialMove> GenAskWhyGoal(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == partner
                                    select new SocialMove("askWhyGoal#", new List<string> { ((WorldFactGoal)fact).goal.name}, 
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenAskWhyAction(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == partner //I may take out this condition in the long run
                                    select new SocialMove("askWhyAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                           mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenTellAction()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId != partner
                                    where ((WorldFactEvent)fact).action.Action.FeatureId != partner
                                    select new SocialMove("tellAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenAskAboutAction()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == partner
                                    select new SocialMove("askAboutAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenTellPreference()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactPreference
                                    where ((WorldFactPreference)fact).person != partner
                                    select new SocialMove("tellPreference#", new List<string> { fact.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenConfirmGoal(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == partner
                                    select new SocialMove("confirmGoal#", new List<string> { ((WorldFactGoal)fact).goal.name },
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> RemoveAlreadyAskedQuestions(List<SocialMove> moves)
    {
        List<string> repeatableActions = new List<string>() { "console", "congratulate", "acknowledge", "nice" };

        List<SocialMove> finalList = new List<SocialMove>();

        foreach(SocialMove move in moves) {
            if (repeatableActions.Contains(move.verb)) {
                finalList.Add(move);
                continue;
            }

            bool alreadyAsked = false;
            foreach (SocialMove previous in executedMoves) {
                if(move.ToString() == previous.ToString()) {
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


    bool FactsMatch(List<WorldFact> facts1, List<WorldFact> facts2)
    {
        if (facts1.Count != facts2.Count) return false;

        foreach(WorldFact fact1 in facts1) {
            bool foundMatch = false;
            foreach(WorldFact fact2 in facts2) {
                if (fact1.Equals(fact2)) {
                    foundMatch = true;
                    break;
                }
            }
            if (!foundMatch) return false;
        }

        return true;
    }

    FoodItem GetFavoriteFoodFromMenu()
    {
        FoodItem favorite = null;
        int maxPoints = -100;

        foreach(FoodItem food in barMenu) {

            int points = ScoreFood(food);

            if (points >= maxPoints) {
                favorite = food;
                maxPoints = points;
            }
        }
        return favorite;
    }


    int ScoreFood(FoodItem food)
    {
        int points = 0;
        switch (speaker.townieInformation.ItemPreference(food.name)) {
            case PreferenceLevel.loved:
                points += 5;
                break;
            case PreferenceLevel.liked:
                points += 2;
                break;
            case PreferenceLevel.disliked:
                points -= 2;
                break;
            case PreferenceLevel.hated:
                points -= 5;
                break;
        }

        Dictionary<PreferenceLevel, List<string>> ingredientsOfInterest = GetOpinionIngredients(food);

        points += ingredientsOfInterest[PreferenceLevel.loved].Count * 3;
        points += ingredientsOfInterest[PreferenceLevel.liked].Count;
        points -= ingredientsOfInterest[PreferenceLevel.disliked].Count;
        points -= ingredientsOfInterest[PreferenceLevel.hated].Count * 3;

        return points;
    }

    Dictionary<PreferenceLevel, List<string>> GetOpinionIngredients(FoodItem food)
    {
        Dictionary<PreferenceLevel, List<string>> dict = new Dictionary<PreferenceLevel, List<string>>() {
            {PreferenceLevel.loved, new List<string>() },
            {PreferenceLevel.liked, new List<string>() },
            {PreferenceLevel.disliked, new List<string>() },
            {PreferenceLevel.hated, new List<string>() }
        };

        foreach (string ingredient in food.ingredients) {
            PreferenceLevel level = speaker.townieInformation.ItemPreference(ingredient);
            if (level == PreferenceLevel.neutral) continue;

            dict[level].Add(ingredient);
        }

        return dict;
    }

    FoodItem GetFromMenu(string name)
    {
        foreach(FoodItem food in barMenu) {
            if (food.name == name) return food;
        }

        return null;
    }

}
