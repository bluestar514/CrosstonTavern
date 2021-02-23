using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationEngine
{
    public Townie speaker;
    List<SocialMove> socialMoves;
    Person partnerPerson;
    string partner;

    List<FoodItem> barMenu;

    bool hasOrdered = false;
    FoodItem foodItem = null;
    List<SocialMove> executedMoves = new List<SocialMove>();


    int turns = 0;

    public int MaxTurns { get; private set; } = 5;

    List<SocialMove> hangingOptions = new List<SocialMove>();

    string patronGeneralMood = "neutral";

    public ConversationEngine(Townie patron, Person partner, List<FoodItem> barMenu)
    {
        speaker = patron;
        partnerPerson = partner;
        this.partner = partner.id;
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

                if( scoreOfRecomended >= Mathf.Abs(scoreOfFavorite / 4)) {
                    return new SocialMove("order#OnRecomendation", new List<string>() { recomendedFood.name });
                }else return new SocialMove("order#OffRecomendation", new List<string>() { favoriteFood.name });
            case "serveOrder#":
                FoodItem servedFood = GetFromMenu(prompt.arguements[0]);

                Dictionary<PreferenceLevel, List<string>> ingredientOpinions = GetOpinionIngredients(servedFood);
                PreferenceLevel opinionOfDish = speaker.townieInformation.ItemPreference(servedFood.name);

                

                if(opinionOfDish != PreferenceLevel.neutral) 
                    facts.Add(new WorldFactPreference(speaker.Id, opinionOfDish, servedFood.name));
                foreach(PreferenceLevel level in ingredientOpinions.Keys) {
                    foreach(string ingredient in ingredientOpinions[level]) {
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

            case "suggest":
                return new SocialMove("passOpenSuggestions", arguements: prompt.arguements, mentionedFacts:prompt.mentionedFacts);
            case "suggest#":
                AskQuestion();
                return new SocialMove("askConfirmSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);
            case "confirmSuggestion#":
                if (prompt.mentionedFacts[0] is WorldFactPotentialAction potentialAction) {
                    BoundAction suggestedAction = potentialAction.action;

                    speaker.gm.AddModule(new GoalModule(new List<GM_Precondition>(),
                                                        new List<Goal>() {
                                                            new GoalAction(suggestedAction, 5)
                                                        },
                                                        "You told me it was a good idea.",
                                                        name: "suggested action",
                                                        timer: 3
                                                        )
                        );
                }

                return new SocialMove("acceptSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);

            case "cancelSuggestion#":
                return new SocialMove("acceptCancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);

            case "askRelationWith":
                return new SocialMove("passOpenAskRelationWith");
            case "askRelationWith#":
                return SocialMoveFactory.MakeMove("tellRelationWith#", speaker, prompt);

            case "askWhyGoal":
                return new SocialMove("passOpenAskWhyGoal");
            case "askWhyGoal#":
                return SocialMoveFactory.MakeMove("tellWhyGoal#", speaker, prompt);

            case "turnsUp":
                MaxTurns = 100;

                List<Goal> goals = speaker.gm.GetGoalsList();

                if(goals != null &&
                    goals.Count > 0 &&
                    foodItem != null &&
                    goals.Any(goal => goal is GoalState goalState &&
                                   goalState.state is StateInventory invState &&
                                   invState.itemId == foodItem.name)) {
                    return new SocialMove("askForRecipe#", arguements: new List<string>() { foodItem.name });
                }

                return new SocialMove("goodbye");
            case "giveRecipe#":
                Feature homeKitchen = speaker.ws.map.GetFeature("kitchen_" + speaker.homeLocation);
                if (homeKitchen == null) homeKitchen = speaker.ws.map.GetFeature("kitchen_inn");

                GenericAction recipe = ActionInitializer.GetAllActions()[foodItem.verb.verbPresent + "_" + foodItem.name];

                homeKitchen.providedActions.Add(recipe);

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

                if(suggestedActions.Count > 0) {
                    moves.Add(new SocialMove("suggest", arguements: prompt.arguements, 
                        mentionedFacts: new List<WorldFact>( from action in suggestedActions
                                                             select new WorldFactPotentialAction(action))));
                }

                moves.Add(new SocialMove("nice"));
                break;
            case "passOpenSuggestions":

                moves.AddRange(from fact in prompt.mentionedFacts
                               select new SocialMove("suggest#", arguements: new List<string>() { fact.ToString() },
                               mentionedFacts: new List<WorldFact>() {fact}));
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

    List<BoundAction> GenSuggestedAction(WorldFact fact)
    {
        ActionBuilder ab = new ActionBuilder(speaker.ws, partnerPerson);

        List<BoundAction> possibleActions = ab.GetAllActions(respectLocation: false);

        System.Func<BoundAction, bool> filter = action=> true;

        if(fact is WorldFactGoal factGoal) {
            Goal goal = factGoal.goal;

            if (goal is GoalAction) return new List<BoundAction>() { };

            GoalState goalState = (GoalState)goal;

            State state = goalState.state;

            if(state is StateInventory stateInv) {
                filter = action => {
                    return (speaker.ws.map.GetFeature(action.FeatureId).type != Feature.FeatureType.person &&
                            speaker.ws.map.GetFeature(action.FeatureId).type != Feature.FeatureType.door) ||
                            (action.Id.StartsWith("ask_") && action.Id.Contains(stateInv.itemId));
                };
            }
            if(state is StateSocial stateSocial) {
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

    List<SocialMove> GenAskOpinionOfPerson(List<string> people=null)
    {
        if (people == null) people = speaker.townieInformation.relationships.GetKnownPeople();

        return new List<SocialMove>(from person in people
                                    where person != speaker.Id && person != "barkeep" && person != partner
                                    select new SocialMove("askRelationWith#", new List<string>() { person }));
    }

    List<SocialMove> RemoveAlreadyAskedQuestions(List<SocialMove> moves)
    {
        List<string> repeatableActions = new List<string>() { "console", "congratulate", "acknowledge", "nice", "suggest", "nevermind", "askRelationWith" };

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
            return "<"+ string.Join(",", new List<string>() { verb, actor, feature }) + ">";
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
