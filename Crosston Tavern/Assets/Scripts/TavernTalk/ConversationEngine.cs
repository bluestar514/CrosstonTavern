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
                facts.Add(new WorldFactPreference(speaker.Id, opinionOfDish, servedFood.name));
                foreach(PreferenceLevel level in ingredientOpinions.Keys) {
                    foreach(string ingredient in ingredientOpinions[level]) {
                        facts.Add(new WorldFactPreference(speaker.Id, level, ingredient));
                    }
                }

                return new SocialMove("thank", mentionedFacts: facts);

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
                return moves;

            case "askForRecomendation":
                foreach(FoodItem item in barMenu) {
                    moves.Add(new SocialMove("recomend#", new List<string>() { item.name }));
                }
                return moves;

            case "order#":
            case "order#OnRecomendation":
            case "order#OffRecomendation":
                moves.Add(new SocialMove("serveOrder#", prompt.arguements));
                return moves;

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
                moves.AddRange(GenAskWhyGoal());
                moves.AddRange(GenAskWhyAction());
                moves.AddRange(GenTellAction());
                moves.AddRange(GenTellPreference());
                //moves.AddRange(GenAskAboutAction());
                return moves;
        }

        
    }


    public void LearnFromInput(SocialMove prompt)
    {
        foreach(WorldFact fact in prompt.mentionedFacts) {
            speaker.ws.LearnFact(fact);
        }
    }


    List<SocialMove> GenAskWhyGoal()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
                                    where fact is WorldFactGoal
                                    select new SocialMove("askWhyGoal#", new List<string> { ((WorldFactGoal)fact).goal.name}, 
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }
    List<SocialMove> GenAskWhyAction()
    {
        return new List<SocialMove>(from fact in speaker.ws.knownFacts.GetFacts()
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
