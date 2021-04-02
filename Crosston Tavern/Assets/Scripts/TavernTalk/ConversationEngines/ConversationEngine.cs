using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationEngine
{
    public Townie speaker;

    protected List<SocialMove> executedMoves = new List<SocialMove>();

    protected List<FoodItem> barMenu;


    public void DoMove(SocialMove move)
    {
        executedMoves.Add(move);
    }

    public List<WorldFact> LearnFromInput(List<WorldFact> mentionedFacts, List<WorldFact> retractedFacts )
    {
        List<WorldFact> learnedFacts = new List<WorldFact>();

        foreach(WorldFact fact in mentionedFacts) {
            learnedFacts.AddRange(speaker.ws.LearnFact(fact));
        }

        foreach (WorldFact fact in retractedFacts) {
            speaker.ws.ForgetFact(fact);
        }

        return learnedFacts;
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

    protected FoodItem GetFavoriteFoodFromMenu()
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


    protected int ScoreFood(FoodItem food)
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

    protected Dictionary<PreferenceLevel, List<string>> GetOpinionIngredients(FoodItem food)
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

    protected FoodItem GetFromMenu(string name)
    {
        foreach(FoodItem food in barMenu) {
            if (food.name == name) return food;
        }

        return null;
    }



}
