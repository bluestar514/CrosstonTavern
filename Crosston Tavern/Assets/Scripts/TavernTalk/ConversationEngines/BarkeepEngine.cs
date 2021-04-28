using System.Collections;
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
        SocialMenu subMenu;
        if (prompt is SocialMenu menu) {
            moves.AddRange(menu.menuOptions);
        } else {

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
                    if (prompt.mentionedFacts.Count > 0) {
                        moves.AddRange(GenAskWhyAction(prompt.mentionedFacts));
                        moves.AddRange(GenAskWhyGoal(prompt.mentionedFacts));
                    }

                    moves.Add(new SocialMove("nice"));
                    break;

                case "tellAboutGoals":
                    if (prompt.mentionedFacts.Count > 0) {
                        moves.AddRange(GenAskWhyAction(prompt.mentionedFacts));
                        moves.AddRange(GenAskWhyGoal(prompt.mentionedFacts));
                    }
                    moves.Add(new SocialMove("askAboutGoalFrustration"));
                    moves.Add(new SocialMove("nice"));
                    break;


                case "frustratedByGoals":
                case "tellWhyGoal#":
                    SocialMenu suggestionMenu = GenSuggestMoves();
                    moves.Add(suggestionMenu);
                    suggestionMenu.AddPreviousContext(new SocialMenu("nevermind", moves));

                    moves.Add(new SocialMove("nice"));
                    break;

                case "makeRoomForPlayerGoal#":
                    List<SocialMove> previousGoals = GenStopGoal();

                    moves.AddRange( from goal in previousGoals
                                    select new CompoundSocialMove("changeGoal#->#", 
                                                                    arguements: new List<string>(){
                                                                        goal.arguements[0],
                                                                        prompt.arguements[0]
                                                                    }, 
                                                                    socialMoves: new List<SocialMove>() {
                                                                        goal, 
                                                                        new SocialMove("suggest_#", prompt.arguements, prompt.mentionedFacts)
                                                                    }
                                                                  )
                                    );
                    moves.Add(new SocialMove("cancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts));
                    break;



                case "askHowToDo#":
                    moves.Add(new SocialMove("tellHowToDo#", prompt.arguements, prompt.mentionedFacts));
                    moves.Add(new SocialMove("cancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts));
                    break;
                case "askConfirmSuggestion#":
                    moves.Add(new SocialMove("confirmSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts));
                    moves.Add(new SocialMove("cancelSuggestion#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts));
                    break;

                case "tellRelationWith#":
                    subMenu = new SocialMenu("openTellRelationWith#", 
                                            GenTellRelationshipGuess(prompt.arguements[0]), 
                                            arguements: prompt.arguements);
                    moves.Add(subMenu);
                    moves.Add(new SocialMove("nice"));
                    subMenu.AddPreviousContext(new SocialMenu("nevermind", moves));
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
                    new SocialMove("askAboutGoalFrustration"),
                    //new SocialMove("askAboutDayFull"),
                    
                    //new SocialMove("askAboutDayHighlights", arguements: new List<string>(){ patronGeneralMood }),
                    //new SocialMove("askAboutObservation"),
                    //new SocialMove("askAboutExcitement"),
                    //new SocialMove("askAboutDisapointment"),
                    //new SocialMove("askAboutPreferencesLike"),
                    //new SocialMove("askAboutPreferencesHate")
                };

                switch (patronGeneralMood) {
                    case "happy":
                            barkeeperMoves.Add(new SocialMove("askAboutExcitement"));
                        break;
                    case "sad":
                    case "angry":
                            barkeeperMoves.Add(new SocialMove("askAboutDisapointment", arguements: new List<string>() { patronGeneralMood }));
                            break;
                    default:
                            barkeeperMoves.Add(new SocialMove("askAboutDayHighlights"));
                        break;
                }

                    moves = new List<SocialMove>(barkeeperMoves);
                    List<SocialMenu> menus = new List<SocialMenu>();
                    

                    List<WorldFact> facts = speaker.ws.knownFacts.GetFacts();

                    facts = TrimOldEvents(facts);
                    facts = TrimSimilarFacts(facts);


                    AddSubmenu("askRelationWith", GenAskOpinionOfPerson(), moves, menus);
                    AddSubmenu("whyGoalMenu", GenAskWhyGoal(facts), moves, menus);
                    AddSubmenu("whyActionMenu", GenAskWhyAction(facts), moves, menus);
                    AddSubmenu("tellActionMenu", GenTellAction(), moves, menus);
                    AddSubmenu("tellPreferenceMenu", GenTellPreference(), moves, menus);
                    AddSubmenu("confirmGoalMenu", GenConfirmGoal(facts), moves, menus);
                    AddSubmenu("stopPlayerGivenGoal", GenStopGoal(), moves, menus);
                    AddSubmenu("askAboutPlayerGivenGoal", GenAskAboutPlayerGivenGoal(), moves, menus);


                    SocialMenu suggestMenu = GenSuggestMoves();
                    menus.Add(suggestMenu);
                    moves.Add(suggestMenu);


                    //moves.AddRange(GenSuggestedAction());
                    //moves.AddRange(GenAskAboutAction());


                    SocialMove nevermind = new SocialMenu("nevermind", moves);
                    
                    foreach(SocialMenu submenu in menus) {
                        submenu.AddPreviousContext(nevermind);
                    }
                    

                    break;
            }
        }

        return RemoveAlreadyAskedQuestions(moves);

    }

    void AddSubmenu(string name, List<SocialMove> submenuOptions, 
        List<SocialMove> overallMovesList, List<SocialMenu> overallSubmenusList)
    {
        submenuOptions = RemoveAlreadyAskedQuestions(submenuOptions);

        if (submenuOptions.Count > 0) {
            SocialMenu submenu = new SocialMenu(name, submenuOptions);
            overallMovesList.Add(submenu);
            overallSubmenusList.Add(submenu);
        }
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
                                                                         complexFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenConfirmGoal(List<WorldFact> facts)
    {
        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == patronId
                                    select new SocialMove("confirmGoal#", new List<string> { ((WorldFactGoal)fact).goal.name },
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenStopGoal(string tag = "stopPlayerGivenGoal#")
    {
        List<WorldFact> facts = speaker.ws.knownFacts.GetFacts().FindAll(fact => {
            return fact is WorldFactGoal goalFact &&
                goalFact.modifier.Contains(WorldFactGoal.Modifier.player);
        });


        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == patronId
                                    select new SocialMove(tag, new List<string> { ((WorldFactGoal)fact).goal.name },
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenAskAboutPlayerGivenGoal()
    {
        List<WorldFact> facts = speaker.ws.knownFacts.GetFacts().FindAll(fact => {
            return fact is WorldFactGoal goalFact &&
                goalFact.modifier.Contains(WorldFactGoal.Modifier.player);
        });


        return new List<SocialMove>(from fact in facts
                                    where fact is WorldFactGoal
                                    where ((WorldFactGoal)fact).owner == patronId
                                    select new SocialMove("askAboutPlayerGivenGoal#", new List<string> { ((WorldFactGoal)fact).goal.name },
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    SocialMenu GenSuggestMoves(List<WorldFact> fact = null)
    {
        ActionBuilder ab = new ActionBuilder(speaker.ws, patron);

        List<BoundAction> possibleActions = ab.GetAllActions(respectLocation: false);

        Dictionary<string, List<BoundAction>> verbToAllActions = new Dictionary<string, List<BoundAction>>();
        foreach(BoundAction possibleAction in possibleActions) {
            string key = possibleAction.Id;

            if (possibleAction.FeatureId == "barkeep") continue;
            if (possibleAction.FeatureId.Contains("door")) continue;

            if (speaker.ws.map.GetFeature(possibleAction.FeatureId).type == Feature.FeatureType.kitchen) {
                if(!possibleAction.FeatureId.Contains(patron.id)) continue;

                key = "cook";
            }
            if (GoalManager.ContainsStaticConditions(
                    possibleAction.preconditions.conditions, possibleAction.Bindings, patronId)) continue;

            if (!verbToAllActions.ContainsKey(key)) {
                verbToAllActions.Add(key, new List<BoundAction>());
            }
            verbToAllActions[key].Add(possibleAction);
        }

        List<SocialMove> submenus = new List<SocialMove>();
        foreach(string verb in verbToAllActions.Keys) {
            


            if (verbToAllActions[verb].Count > speaker.ws.map.GetPeople().Count() * 3) {
                //If there are a lot of actions is a given submenu, it is probably full of actions aimed at 
                // several people, using different items (gift giving for example
                // so split that up between additional submenus
                
                Dictionary<string, List<BoundAction>> targetToActions = new Dictionary<string, List<BoundAction>>();
                foreach (BoundAction action in verbToAllActions[verb]) {

                    if (!targetToActions.ContainsKey(action.FeatureId)) {
                        targetToActions.Add(action.FeatureId, new List<BoundAction>());
                    }
                    targetToActions[action.FeatureId].Add(action);
                }

                if (targetToActions.Keys.Count > 1) {
                    //If verb is something like "give_#item#" with lots of suboptions per person...

                    //Make each person a submenu with all their potential gifts
                    List<SocialMove> peopleSubMenus = new List<SocialMove>();
                    foreach (string personName in targetToActions.Keys) {

                        peopleSubMenus.Add(MakeSubmenu(targetToActions[personName], 
                                                        MakeSubmenuLable(verbToAllActions[verb][0], personName)));
                    }

                    AddNevermind(peopleSubMenus);

                    SocialMenu submenu = new SocialMenu("suggest_#",
                                            peopleSubMenus,
                                            mentionedFacts: new List<WorldFact>() { MakeSubmenuLable(verbToAllActions[verb][0]) });
                    submenus.Add(submenu);

                } else {

                    submenus.Add(MakeSubmenu(verbToAllActions[verb], 
                                            MakeSubmenuLable(verbToAllActions[verb][0])));
                }

            } else {

                submenus.Add(MakeSubmenu(verbToAllActions[verb], 
                                          MakeSubmenuLable(verbToAllActions[verb][0])));
            }

        }


        SocialMenu suggestMenu = new SocialMenu("suggest", submenus);
        AddNevermind(submenus);

        return suggestMenu;
    }

    SocialMenu MakeSubmenu(List<BoundAction> actions, WorldFact menuName)
    {
        List<SocialMove> subMenuOptions = new List<SocialMove>();

        //Make Submenu based on Action Verb (ex: fishing) containing 
        //all actions taking place at any feature that supports it
        //(ex: fishing at lake, fishing at farm_river)
        foreach (BoundAction action in actions) {
            subMenuOptions.Add(new SocialMove("suggest_#",
                                                arguements: new List<string>() { action.ToString() },
                                                mentionedFacts: new List<WorldFact>() {
                                                                    new WorldFactPotentialAction(action) }
                                              ));
        }
        return  new SocialMenu("suggest_#", subMenuOptions,
                                                mentionedFacts: new List<WorldFact>() { 
                                                    menuName
                                                });



    }

    WorldFact MakeSubmenuLable(BoundAction baseAction, string specifyFeature="...", string specifyLocation = "...")
    {
        List<BoundBindingPort> bindings = new List<BoundBindingPort>();
        foreach(BoundBindingPort port in baseAction.Bindings.bindings) {
            bindings.Add(new BoundBindingPort(port.tag, "..."));
        }

        return 
            new WorldFactPotentialAction(
                new BoundAction(baseAction,
                                baseAction.ActorId,
                                specifyFeature,
                                specifyLocation,
                                new BoundBindingCollection(bindings),
                                baseAction.verbilizationInfo)
                );
    }

    void AddNevermind(List<SocialMove> submenus)
    {
        submenus.ForEach(option =>
            {
                if (option is SocialMenu submenu) {
                    submenu.AddPreviousContext(new SocialMenu("nevermind", submenus));
                }
            }
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
                                                            complexFacts: new List<WorldFact>() {
                                                                new WorldFactRelation(new StateRelation(target, patronId, relation), patronId)
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
