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
                FoodItem food = GetFavoriteFoodFromMenu();

                return new SocialMove("order#", new List<string>() { food.name });
            case "askAboutGoals":
                return SocialMoveFactory.MakeTellAboutGoals(speaker);
            case "askAboutDayFull":
                return SocialMoveFactory.MakeTellAboutDayEvents(speaker);
            case "askAboutDayHighlights":
                return SocialMoveFactory.MakeTellAboutInterestingEvents(speaker, Opinion.Tag.noteworthy);
            case "askAboutObservation":
                return SocialMoveFactory.MakeTellAboutObservedEvents(speaker);
            case "askWhyGoal#":
                return SocialMoveFactory.MakeTellWhyGoal(speaker, prompt);
            case "askWhyAction#":
                return SocialMoveFactory.MakeTellWhyAction(speaker, prompt);
            case "askAboutExcitement":
                return SocialMoveFactory.MakeTellAboutInterestingEvents(speaker, Opinion.Tag.excited);
            case "askAboutDisapointment":
                return SocialMoveFactory.MakeTellAboutInterestingEvents(speaker, Opinion.Tag.disapointed);
            case "askAboutAction#": //Currently not used, consider removing
                return new SocialMove("tellDetailsOfAction#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);
            case "askAboutPreferencesLike":
                return  SocialMoveFactory.MakeTellPreference(speaker, true);
            case "askAboutPreferencesHate":
                return  SocialMoveFactory.MakeTellPreference(speaker, true);
            case "tellAction#":
            case "tellPreference#":
                return new SocialMove("acknowledge", mentionedFacts: prompt.mentionedFacts);
            default:
                return new SocialMove("DEFAULT");
        }
    }

    public List<SocialMove> GetSocialMoves(SocialMove prompt)
    {
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

        List<SocialMove> moves = new List<SocialMove>(barkeeperMoves);
        moves.AddRange(GenAskWhyGoal());
        moves.AddRange(GenAskWhyAction());
        moves.AddRange(GenTellAction());
        moves.AddRange(GenTellPreference());
        //moves.AddRange(GenAskAboutAction());
        return moves;
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
            int points = 0;

            switch(speaker.townieInformation.ItemPreference(food.name)) {
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

            foreach (string ingredient in food.ingredients) {
                switch (speaker.townieInformation.ItemPreference(ingredient)) {
                    case PreferenceLevel.loved:
                        points += 3;
                        break;
                    case PreferenceLevel.liked:
                        points += 1;
                        break;
                    case PreferenceLevel.disliked:
                        points -= 1;
                        break;
                    case PreferenceLevel.hated:
                        points -= 3;
                        break;
                }
            }

            if (points >= maxPoints) {
                favorite = food;
                maxPoints = points;
            }
        }
        return favorite;
    }

}
