using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationVerbalizer 
{
    public Townie townie;
    string partner;
    Verbalizer v;

    public ConversationVerbalizer(Townie townie, string partner)
    {
        this.townie = townie;
        v = new Verbalizer(townie.townieInformation.id, partner, townie.ws);
        this.partner = partner;
    }

    public DialogueUnit ExpressSocialMove(SocialMove socialMove)
    {

        string verbalization = socialMove.ToString();
        NPCPortrait.State emotion = NPCPortrait.State.neutral;

        switch(townie.townieInformation.statusEffectTable.GetStrongestStatus()) {
            case EntityStatusEffectType.happy:
                emotion = NPCPortrait.State.happy;
                break;
            case EntityStatusEffectType.angry:
                emotion = NPCPortrait.State.angry;
                break;
            case EntityStatusEffectType.sad:
                emotion = NPCPortrait.State.sad;
                break;
        }
        KeyValuePair<string, NPCPortrait.State> pair;
        string actionName;
        string str;
        ExecutedAction actionObj;
        List<WorldFact> facts = socialMove.mentionedFacts;
        List<string> goals = new List<string>();
        WorldFactGoal goalFact;
        WorldFactPotentialAction actionFact;
        switch (socialMove.verb) {
            case "greet":
                verbalization = "Good evening.";
                break;

            case "askAboutState":
                verbalization = "How are you doing?";
                break;
            case "askForOrder":
                verbalization = "What can I get you this evening?";
                break;
            case "tellStateNONE":    
            case "tellState#":
                string state;
                if (socialMove.arguements.Count == 0) state = "none";
                else state = socialMove.arguements[0];

                if(state == EntityStatusEffectType.angry.ToString()) {
                    verbalization = "I have had a real frustrating day!";
                    emotion = NPCPortrait.State.angry;
                } else if (state == EntityStatusEffectType.sad.ToString()) {
                    verbalization = "Nothing went right today...";
                    emotion = NPCPortrait.State.sad;
                } else if (state == EntityStatusEffectType.happy.ToString()) {
                    verbalization = "I had a great day!";
                    emotion = NPCPortrait.State.happy;
                } else {
                    verbalization = "I'm doing alright. You know how it is.";
                }

                socialMove.mentionedFacts = new List<WorldFact>();
                break;
            case "congratulate":
                verbalization = "That's good!";
                break;
            case "console":
                verbalization = "Sorry to hear that";
                break;

            case "askForRecomendation":
                verbalization = "What's good on the menu tonight?";
                break;
            case "recomend#":
                str = VerbalizationDictionary.Replace(socialMove.arguements[0]);

                verbalization = "The " + str + " is quite good.";
                break;
            case "order#OnRecomendation":
                verbalization = "Sound good, let's go with that tonight.";
                emotion = NPCPortrait.State.happy;

                break;
            case "order#OffRecomendation":
                str = VerbalizationDictionary.Replace(socialMove.arguements[0]);

                verbalization = "Mmm, no. I don't think I'm feeling that tonight. How about " +
                                    str + " instead?";
                break;
            case "order#":
                str = VerbalizationDictionary.Replace(socialMove.arguements[0]);

                verbalization = "Can I get a " + str + "?";
                break;
            case "serveOrder#":
                verbalization = "Coming right up.";
                break;
            case "thank":
                //Thanks. This is good. 
                //Thanks. Oh man, I love strawberry cake. Actually, I like anything with strawberries. 
                //Thanks. This was a good choice, I like anything with strawberries.
                emotion = NPCPortrait.State.soup;

                pair = VerbalizeDishThank(socialMove);

                verbalization = pair.Key;
                if (pair.Value != NPCPortrait.State.none) emotion = pair.Value;
                break;


            case "askAboutDayHighlights":

                verbalization = "Anything interesting happen today?";


                break;
            case "askAboutObservation":
                verbalization = "Did you see anything interesting today?";
                break;
            case "askAboutExcitement":
                verbalization = "So, what happened today that's got you in such a good mood?";
                break;
            case "askAboutDisapointment":
                string patronGeneralMood = socialMove.arguements[0];

                switch (patronGeneralMood) {
                    case "sad":
                        verbalization = "You want to talk about what's got you so upset today?";
                        socialMove.verb = "askAboutDisapointment";
                        break;
                    case "angry":
                        verbalization = "You want to tell me what's got you in such a bed mood today?";
                        socialMove.verb = "askAboutDisapointment";
                        break;
                    default:
                        verbalization = "Anything interesting happen today?";
                        break;
                }
                break;
            case "askAboutGoals":
                verbalization = "What have you been trying to do lately?";
                break;
            case "askAboutGoalFrustration":
                verbalization = "What have you been having trouble with lately?";
                break;

            case "askWhyAction#":
                //Why did you go fishing yesterday?


                actionName = socialMove.arguements[0];
                actionObj = townie.ws.knownFacts.GetActionFromName(actionName);

                verbalization = v.VerbalizeActionWithDate(actionObj, true);
                verbalization = "Why did " + verbalization + "?";
                break;
            case "askAboutPreferencesLike":
                verbalization = "What do you like?";
                break;
            case "askAboutPreferencesHate":
                verbalization = "What do you dislike?";
                break;
            case "tellAction#":
                actionName = socialMove.arguements[0];
                actionObj = townie.ws.knownFacts.GetActionFromName(actionName);

                verbalization = v.VerbalizeActionWithResults(actionObj, false);
                verbalization = "Did you hear that " + verbalization + "?";
                break;
            case "tellAboutDayEvents":
            case "tellAbout#Event":
            case "tellAboutDayObservedEvents":
                //float coin = Random.value;
                //if(coin > .5)
                    verbalization = "Today, "+ VerbalizeAllEvents(facts);
                //else
                //    verbilization = VerbalizeByTimePeriod(facts);

                if(socialMove.arguements.Count > 0) {
                    if(socialMove.arguements[0] == "EXCITED") {
                        emotion = NPCPortrait.State.happy;
                    }
                    if(socialMove.arguements[0] == "DISAPOINTED") {
                        emotion = NPCPortrait.State.sad;
                    }
                }

                break;
            case "tellWhyGoal#TopLevel":
                if (facts.Count <= 1) {
                    verbalization = "I just do.";
                    break;
                } else {
                    if (facts[0] is WorldFactGoal subjectGoalFact) {

                        List<WorldFact> reasonsForSubjectGoal = facts;
                        reasonsForSubjectGoal.RemoveAt(0);

                        verbalization = "";

                        List<string> reasons = new List<string>();
                        foreach (WorldFact fact in reasonsForSubjectGoal) {
                            if (fact is WorldFactGoalModulePrecondition factPre) {
                                GM_Precondition precondition = factPre.precondition;

                                reasons.Add(precondition.Verbalize(townie.Id, partner, townie.ws));
                            }
                        }

                        verbalization += " " + Verbalizer.MakeNiceList(reasons);
                    }


                }

                break;
            case "tellWhyGoal#":
                /*mentionedFacts:   0 - goal we are talking about,
                                    1 - an action unlocked by completing this goal, 
                                    2 + reasons for goal
                */
                if (facts.Count <= 1) {
                    verbalization = "I just do.";
                    break;
                } else {

                    
                    if (facts[0] is WorldFactGoal subjectGoalFact &&
                        facts[1] is WorldFactPotentialAction potentialDesirableAction) {

                        List<WorldFact> reasonsForSubjectGoal = facts;
                        reasonsForSubjectGoal.RemoveAt(1);
                        reasonsForSubjectGoal.RemoveAt(0);
                        


                        verbalization = "I want to" + v.VerbalizeAction(potentialDesirableAction.action, true, false);

                        List<string> reasons = new List<string>();
                        foreach(WorldFact fact in reasonsForSubjectGoal) {
                            if(fact is WorldFactGoal parentGoalFact) {
                                Goal parentGoal = parentGoalFact.goal;

                                reasons.Add(v.VerbalizeGoal(parentGoal));
                            }
                        }

                        verbalization += " " + Verbalizer.MakeNiceList(reasons)+".";
                    }

                   
                }

                break;
            case "tellAboutGoals":
                List<string> motivations = new List<string>();

                //I want to have 3 to 1000 trout
                //I want to be friendlier with Alicia
                //I want to be dating Alicia
                //I want Alicia to have 4 to 5 strawberry
                foreach (WorldFact fact in facts) {
                    if(fact is WorldFactPotentialAction) {
                        actionFact = (WorldFactPotentialAction)fact;
                        motivations.Add(v.VerbalizeAction(actionFact.action, true, false));
                    }
                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizeGoal(goalFact.goal));
                    }
                }
                verbalization = "";
                if (motivations.Count > 0) {
                    verbalization = "I want " + Verbalizer.MakeNiceList(motivations);
                    verbalization += ", so I can " + Verbalizer.MakeNiceList(goals)+".";
                } else {
                    verbalization = verbalization + "I want " + string.Join(". I want ", goals) + ". ";
                }
                break;
            case "tellWhyAction#":
                foreach (WorldFact fact in facts) {
                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizeGoal(goalFact.goal));
                    }
                }

                verbalization = string.Join(", and so I wanted ", goals);
                verbalization = "I wanted " + verbalization;
                break;
            case "askWhyGoal#":
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;

                        string snippet = v.VerbalizeGoal(goalFact.goal);
                        if(snippet.StartsWith("you ")) {
                            snippet = snippet.Remove(0, 4);
                        }

                        goals.Add(snippet);
                    }
                }

                verbalization = "Why do you want " + goals[0] + "?";
                break;
            case "askAboutAction#":
                actionName = socialMove.arguements[0];
                actionObj = townie.ws.knownFacts.GetActionFromName(actionName);

                verbalization = v.VerbalizeAction(actionObj, false);
                verbalization = "Could you tell me more about how " + verbalization + "?";
                break;
            case "tellDetailsOfAction#":
                foreach (WorldFact fact in facts) {
                    if (fact is WorldFactEvent) {

                        WorldFactEvent e = (WorldFactEvent)fact;
                        ExecutedAction a = e.action;

                        foreach (Effect effect in a.executedEffect) {
                            verbalization += effect.ToString();
                        }
                    }
                }


                break;

            case "tellPreference":
            case "tellPreference#":
                if (socialMove.arguements.Count > 0) verbalization = "Did you know ";
                else verbalization = "";
                foreach (WorldFact fact in facts) {
                    verbalization += fact.Verbalize(townie.Id, partner);
                }
                if (socialMove.arguements.Count > 0) verbalization += "?";
                else verbalization += ".";

                break;

            case "confirmGoal#":
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizeGoal(goalFact.goal));
                    }
                }

                verbalization = "Do you still want to "+goals[0] + "?";
                break;
            case "confirmGoal#InDate":
                verbalization = "Yes, I do!";
                break;
            case "confirmGoal#OutOfDate":
                verbalization = "No, I don't.";
                break;


            case "suggest":
                verbalization = "Suggest...";
                break;
            case "suggest#":
                if(socialMove.mentionedFacts[0] is WorldFactPotentialAction factAction) {
                    BoundAction action = factAction.action;
                    verbalization = "Why not " + v.VerbalizeAction(action, true, false);
                }
                
                break;
            case "askConfirmSuggestion#":
                verbalization = "You really think that will help?";
                break;
            case "confirmSuggestion#":
                verbalization = "Yeah, go for it!";
                break;
            case "cancelSuggestion#":
                verbalization = "Actually, no, nevermind...";
                break;
            case "acceptSuggestion#":
                verbalization = "Alright, I guess I'll try doing that tomorrow.";
                break;
            case "acceptCancelSuggestion#":
                verbalization = "Well, alright then.";
                break;

            case "frustratedByGoals":
                verbalization = "I want ";
                List<string> frustrations = new List<string>();
                foreach(WorldFact fact in socialMove.mentionedFacts) {
                    if(fact is WorldFactGoal factGoal &&
                        factGoal.goal is GoalState goalState)
                        frustrations.Add( goalState.state.Verbalize(townie.Id, partner, true));
                }
                verbalization += Verbalizer.MakeNiceList(frustrations);

                verbalization += " but I can't figure out how.";
                break;
            case "askRelationWith":
                verbalization = "What do you think about...";
                break;
            case "askRelationWith#":
                verbalization = "What do you think about " + VerbalizationDictionary.Replace(socialMove.arguements[0])+"?";
                break;
            case "tellRelationWith#":

                pair = VerbalizePeopleOpinion(facts);
                verbalization = pair.Key;
                if (pair.Value != NPCPortrait.State.none) emotion = pair.Value;
                break;
            case "openTellRelationWith#":
                verbalization = "I think...";
                break;
            case "tell#Relation#":

                string subject = "ERROR";
                Relationship.Tag tag = Relationship.Tag.acquantences;
                if( facts.Count > 0 &&
                    facts[0] is WorldFactRelation factRelation ) {
                    tag = factRelation.relation.tag;
                    subject = factRelation.relation.source;
                }

                subject = VerbalizationDictionary.Replace(subject);

                switch (tag) {
                    case Relationship.Tag.bestFriend:
                        verbalization = subject + " thinks of you as a very close friend.";
                        break;
                    case Relationship.Tag.friend:
                        verbalization = subject + " thinks of you as a good friend.";
                        break;
                    case Relationship.Tag.liked:
                        verbalization = subject +  " thinks positively of you.";
                        break;
                    case Relationship.Tag.disliked:
                        verbalization = subject + " thinks negatively of you.";
                        break;
                    case Relationship.Tag.enemy:
                        verbalization = subject + " doesn't like you very much.";
                        break;
                    case Relationship.Tag.nemisis:
                        verbalization = subject + " hates you.";
                        break;
                    case Relationship.Tag.no_affection:
                        verbalization = "I don't think they are romantically interested in you.";
                        break;
                    case Relationship.Tag.crushing_on:
                        verbalization = subject + " might have a crush on you.";
                        break;
                    case Relationship.Tag.in_love_with:
                        verbalization = subject + " is in love with you.";
                        break;
                    case Relationship.Tag.head_over_heels:
                        verbalization = subject + " is deeply in love with you.";
                        break;
                    default:
                        verbalization = "I don't think they have a strong opinion about you.";
                        break;
                }

                break;
            case "whyGoalMenu":
                verbalization = "Why do you...";
                break;
            case "confirmGoalMenu":
                verbalization = "Do you still want to...";
                break;
            case "tellActionMenu":
                verbalization = "Did you hear that...";
                break;

            case "nice":
                verbalization = "I see";
                break;
            case "nevermind":
                verbalization = "Nevermind.";
                break;
            case "acknowledge":
                verbalization = "No, I didn't";
                Debug.LogWarning("No checking is done as to whether " + townie + " has actually heard " + string.Join(",",socialMove.mentionedFacts));
                break;
        }

        return new DialogueUnit(verbalization, townie.name, socialMove, emotion);
    }


    KeyValuePair<string, NPCPortrait.State> VerbalizeDishThank(SocialMove socialMove)
    {
        string verbalization = "";
        NPCPortrait.State emotion = NPCPortrait.State.none;

        List<WorldFact> facts = new List<WorldFact>(socialMove.mentionedFacts);

        if (facts.Count == 0 || !(facts[0] is WorldFactPreference))
            throw new System.Exception("Can't verbalize opinion of dish if opinion was not given with social move!" +
                " (" + socialMove + ")");

        WorldFactPreference opinionOfDish = (WorldFactPreference)socialMove.mentionedFacts[0];
        facts.RemoveAt(0);

        Dictionary<PreferenceLevel, List<string>> prefDict = new Dictionary<PreferenceLevel, List<string>>();

        foreach (WorldFact fact in socialMove.mentionedFacts) {

            if (fact is WorldFactPreference) {
                WorldFactPreference preference = (WorldFactPreference)fact;

                PreferenceLevel level = preference.level;
                string item = VerbalizationDictionary.Replace(preference.item, VerbalizationDictionary.Form.plural);

                if (!prefDict.ContainsKey(level)) prefDict.Add(level, new List<string>());
                prefDict[level].Add(item);
            }
        }

        Dictionary<PreferenceLevel, string> opinions = new Dictionary<PreferenceLevel, string>();

        if (prefDict.ContainsKey(PreferenceLevel.loved)) {
            opinions.Add(PreferenceLevel.loved, 
                    "I absolutely love anything with " +
                    Verbalizer.MakeNiceList(prefDict[PreferenceLevel.loved], false));
        }
        if (prefDict.ContainsKey(PreferenceLevel.liked)) {
            opinions.Add(PreferenceLevel.liked, 
                    "I like stuff with " +
                    Verbalizer.MakeNiceList(prefDict[PreferenceLevel.liked], false));
        }
        if (prefDict.ContainsKey(PreferenceLevel.disliked)) {
            opinions.Add(PreferenceLevel.disliked, 
                    "I don't really like " +
                    Verbalizer.MakeNiceList(prefDict[PreferenceLevel.disliked], false) + 
                    " in my food");
        }
        if (prefDict.ContainsKey(PreferenceLevel.hated)) {
            opinions.Add(PreferenceLevel.hated, 
                    "I hate " +
                    Verbalizer.MakeNiceList(prefDict[PreferenceLevel.hated], true));
        }


        string dishOpinion = "";
        if (facts.Count > 0)
            dishOpinion = opinionOfDish.Verbalize(townie.Id, partner);

        bool loved = opinions.ContainsKey(PreferenceLevel.loved);
        bool liked = opinions.ContainsKey(PreferenceLevel.liked);
        bool dised = opinions.ContainsKey(PreferenceLevel.disliked);
        bool hated = opinions.ContainsKey(PreferenceLevel.hated);

        if(opinionOfDish.level == PreferenceLevel.loved ||
            opinionOfDish.level == PreferenceLevel.liked ||
            opinionOfDish.level == PreferenceLevel.neutral) {
            
            verbalization = "Thanks. ";
            if (opinionOfDish.level == PreferenceLevel.loved) {
                verbalization += "Oh man, " + dishOpinion + ". ";
                emotion = NPCPortrait.State.happy;
            }
            if (opinionOfDish.level == PreferenceLevel.liked) {
                verbalization += dishOpinion + ". ";
            }

            if (loved) verbalization += opinions[PreferenceLevel.loved];
            if (loved && liked) verbalization += " and ";
            if (liked) verbalization += opinions[PreferenceLevel.liked];

            if ((loved || liked) && (dised || hated)) verbalization += ". Although, ";

            if (dised) verbalization += opinions[PreferenceLevel.disliked];
            if (dised && hated) verbalization += " and ";
            if (hated) verbalization += opinions[PreferenceLevel.hated];

            if (loved || liked || dised || hated) verbalization += ".";
        } else {
            if (opinionOfDish.level == PreferenceLevel.disliked) {
                verbalization += "Hmm, " + dishOpinion + ". ";
            }
            if (opinionOfDish.level == PreferenceLevel.liked) {
                verbalization +="Oh. Um. Sorry. I hate this. This is kind of gross.";
                emotion = NPCPortrait.State.sad;
            }

            if (hated) verbalization += opinions[PreferenceLevel.hated];
            if (dised && hated) verbalization += " and ";
            if (dised) verbalization += opinions[PreferenceLevel.disliked];

            if ((loved || liked) && (dised || hated)) verbalization += ". Although, ";

            if (liked) verbalization += opinions[PreferenceLevel.liked];
            if (loved && liked) verbalization += " and ";
            if (loved) verbalization += opinions[PreferenceLevel.loved];

            if (loved || liked || dised || hated) verbalization += ".";
        }


        

        return new KeyValuePair<string, NPCPortrait.State>( verbalization, emotion);
    }

    KeyValuePair<string, NPCPortrait.State> VerbalizePeopleOpinion(List<WorldFact> facts)
    {
        string verbalization = "";
        NPCPortrait.State emotion = NPCPortrait.State.none;

        List<Relationship.Tag> speakerToTargetTags = new List<Relationship.Tag>();
        List<Relationship.Tag> targetToSpeakerTags = new List<Relationship.Tag>();

        Dictionary<Relationship.Axis, int> speakerToTargetFeeling = new Dictionary<Relationship.Axis, int>();
        Dictionary<Relationship.Axis, int> targetToSpeakerFeeling = new Dictionary<Relationship.Axis, int>();

        foreach (WorldFact fact in facts) {
            if (fact is WorldFactRelation factRel &&
                factRel.relation is StateRelation rel) {
                if (rel.source == townie.Id) speakerToTargetTags.Add(rel.tag);
                else targetToSpeakerTags.Add(rel.tag);
            }
            if (fact is WorldFactState factState &&
                factState.state is StateSocial social) {
                if (social.sourceId == townie.Id) speakerToTargetFeeling.Add(social.axis, social.min);
                else targetToSpeakerFeeling.Add(social.axis, social.min);
            }
        }

        if (speakerToTargetTags.Contains(Relationship.Tag.dating)) {
            verbalization += "We're dating";
            if (speakerToTargetTags.Contains(Relationship.Tag.head_over_heels) ||
               speakerToTargetTags.Contains(Relationship.Tag.in_love_with)) {
                verbalization += "! ";
            } else {
                verbalization += ". ";
            }

            emotion = NPCPortrait.State.happy;
        }

        int friendDif = 0;
        int romanceDif = 0;
        if (speakerToTargetTags.Contains(Relationship.Tag.head_over_heels)) {
            verbalization += "I absolutely love them. They're just the best. ";
            romanceDif = 50;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.in_love_with)) {
            verbalization += "I love them. ";
            romanceDif = 20;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.crushing_on)) {
            verbalization += "I think they're really cute. ";
            romanceDif = 10;
        }
        if (speakerToTargetTags.Contains(Relationship.Tag.bestFriend)) {
            verbalization += "We're best friends. ";
            emotion = NPCPortrait.State.happy;
            friendDif = 50;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.friend)) {
            verbalization += "We're friends. ";
            friendDif = 20;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.liked)) {
            verbalization += "They seem like a good person. ";
            friendDif = 10;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.nemisis)) {
            verbalization += "I hate them. They are the absolute worst! ";
            emotion = NPCPortrait.State.angry;
            friendDif = 50;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.enemy)) {
            verbalization += "We don't get along. I don't like them at all. ";
            emotion = NPCPortrait.State.sad;
            friendDif = 20;
        } else if (speakerToTargetTags.Contains(Relationship.Tag.disliked)) {
            verbalization += "I don't like them very much. ";
            friendDif = 10;
        } 
        
        
        if(speakerToTargetTags.Count == 0) {
            verbalization += "They're fine, I guess. ";
        }

        if (speakerToTargetTags.Contains(Relationship.Tag.in_love_with)) {
            if (speakerToTargetFeeling[Relationship.Axis.romantic] >
            targetToSpeakerFeeling[Relationship.Axis.romantic] + romanceDif) {
                verbalization += "But, I'm worried that they might not love me as much as I love them...";
            }
        } else if (speakerToTargetTags.Contains(Relationship.Tag.crushing_on)) {
            if (speakerToTargetFeeling[Relationship.Axis.romantic] >
            targetToSpeakerFeeling[Relationship.Axis.romantic] + romanceDif) {
                verbalization += "But, I don't think they like me the same way...";
            }
        } else if (speakerToTargetTags.Contains(Relationship.Tag.liked)) {
            if (speakerToTargetFeeling[Relationship.Axis.friendly] >
            targetToSpeakerFeeling[Relationship.Axis.friendly] + friendDif) {
                verbalization += "But, I feel like I might like them more than they like me...";
            }
        } else if (speakerToTargetTags.Contains(Relationship.Tag.disliked)) {
            if (speakerToTargetFeeling[Relationship.Axis.friendly] <
            targetToSpeakerFeeling[Relationship.Axis.friendly] + friendDif) {
                verbalization += "But, I don't know why, but they don't seem to dislike me nearly as much.";
            }
        }


        return new KeyValuePair<string, NPCPortrait.State>(verbalization, emotion);
    }

    string VerbalizeAllEvents(List<WorldFact> facts)
    {
        string verbilization = "";
        List<ExecutedAction> allActions = new List<ExecutedAction>();
        
        foreach (WorldFact fact in facts) {
            if (fact is WorldFactEvent) {

                WorldFactEvent e = (WorldFactEvent)fact;
                ExecutedAction a = e.action;
                allActions.Add(a);
            }
        }

        List<string> collectedEvents = new List<string>();
        foreach (ExecutedAction action in allActions) {
            collectedEvents.Add(v.VerbalizeActionWithResults(action, false));
        }

        if (collectedEvents.Count >= 3) {
            string lastevent = collectedEvents.Last();
            collectedEvents.RemoveAt(collectedEvents.Count - 1);

            verbilization += string.Join(", ", collectedEvents) + ", and " + lastevent + ".";
        } else if (collectedEvents.Count == 2) {
            verbilization += string.Join(" and ", collectedEvents) + ".";
        } else if (collectedEvents.Count == 1) {
            verbilization += collectedEvents[0];
        } else {
            verbilization = "nothing interesting happened.";
        }

        return verbilization;
    }

    string VerbalizeByTimePeriod(List<WorldFact> facts)
    {
        List<WorldFact> morniningEvents = new List<WorldFact>();
        List<WorldFact> afternoonEvents = new List<WorldFact>();
        List<WorldFact> eveningEvents = new List<WorldFact>();

        foreach(WorldFact fact in facts) {
            WorldFactEvent e = (WorldFactEvent)fact;
            ExecutedAction a = e.action;
            WorldTime time = a.executionTime;
            if (time < WorldTime.Noon) {
                morniningEvents.Add(e);
            }else if(time < WorldTime.Evening) {
                afternoonEvents.Add(e);
            }else if(time< WorldTime.Night) {
                eveningEvents.Add(e);
            }
        }

        string verbalization = "";
        if(morniningEvents.Count > 0) verbalization = verbalization+ "This morning, " + VerbalizeAllEvents(morniningEvents)+ ". ";
        if (afternoonEvents.Count > 0) verbalization = verbalization + "This afternoon, " + VerbalizeAllEvents(morniningEvents) + ". ";
        if (eveningEvents.Count > 0) verbalization = verbalization + "Then, this evening, " + VerbalizeAllEvents(morniningEvents) + ". ";

        return verbalization;

    }

    List<string> UnrollGoalChain(Goal initGoal)
    {
        List<Goal> goalChain = new List<Goal>() { initGoal };

        int x = 0;
        //List<Goal> backlog = new List<Goal>() { initGoal };

        //while (backlog.Count > 0) {
        //    Goal goal = backlog[0];
        //    backlog.RemoveAt(0);

        //    goalChain.AddRange(goal.GetParentGoals());
        //    backlog.AddRange(goal.GetParentGoals());
        //    Debug.Log("goal: " + goal + " parents: " + goal.GetParentGoals().Count);
        //    x++;
        //    if (x > 1000) throw new System.Exception("Infinite Loop detected!");
        //}

        Goal currentGoal = initGoal;
        while(currentGoal.GetParentGoals().Count > 0) {
            Goal maxPri = currentGoal.GetParentGoals()[0];
            foreach(Goal parent in currentGoal.GetParentGoals()) {
                Debug.Log("goal:" + currentGoal + " parent:" + parent);
                if (parent.priority > maxPri.priority) maxPri = parent;
            }

            goalChain.Add(maxPri);
            currentGoal = maxPri;

            x++;
            if (x > 1000) throw new System.Exception("Infinite Loop detected!");
        }

        goalChain.Reverse();

        List<Goal> noDups = new List<Goal>();
        HashSet<Goal> seen = new HashSet<Goal>();
        foreach (Goal g in goalChain) {
            if (!seen.Contains(g)) {
                noDups.Add(g);
                seen.Add(g);
            }

        }

        List<string> subVerbilizations = new List<string>();
        foreach (Goal goal in noDups) {
            subVerbilizations.Add(v.VerbalizeGoal(goal));
        }

        return subVerbilizations;

    }
}
