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
        string subject = "ERROR";

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
                        verbalization = "You want to tell me what's got you in such a bad mood today?";
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

            case "tellAboutPlayerDirectedEvent":
                //Today I did X, Y, and Z. I'm still having trouble with A, B, and C.
                //I didn't get to it today, but its still on my todo list!

                if (socialMove is CompoundSocialMove cSocialMove)
                    verbalization = TalkAboutPlayerDirectedGoal(cSocialMove);
                else throw new System.Exception("Unexpected input!");



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
                        


                        verbalization = "I want to " + v.VerbalizeAction(potentialDesirableAction.action, true, false);

                        List<string> reasons = new List<string>();
                        foreach(WorldFact fact in reasonsForSubjectGoal) {
                            if(fact is WorldFactGoal parentGoalFact) {
                                Goal parentGoal = parentGoalFact.goal;

                                reasons.Add(v.VerbalizeGoal(parentGoal, true));
                            }
                        }
                        if (reasons.Count != 0) {

                            string lst = Verbalizer.MakeNiceList(reasons);

                            if (!lst.StartsWith("to ")) verbalization += " so ";

                            verbalization += lst;
                        }
 
                        verbalization += ".";
                    }

                   
                }

                break;
            case "tellAboutGoals":
                if (socialMove is CompoundSocialMove compound) {

                    verbalization = VerbalizeGoalTrees(compound);
                } else {
                    throw new System.Exception("Incorrect kind of SocialMove created and passed as a <tellAboutGoals>");
                }


                break;

            case "tellWhyAction#":
                foreach (WorldFact fact in facts) {
                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizeGoal(goalFact.goal));
                    }
                }

                verbalization = Verbalizer.MakeNiceList(goals);
                verbalization = "I wanted " + verbalization;
                break;
            case "askWhyGoal#":
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;

                        string snippet = v.VerbalizeGoal(goalFact.goal);
                        Debug.Log(snippet);

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
                facts = new List<WorldFact>(socialMove.mentionedFacts);
                facts.AddRange(socialMove.complexFacts);
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

            case "suggest_#":
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

            case "acceptChangeGoal#->#":
                if(socialMove is CompoundSocialMove compoundSocialMove) {
                    IEnumerable<DialogueUnit> verbalizations = from move in compoundSocialMove.socialMoves
                                                                select ExpressSocialMove(move);
                    Debug.Log(string.Join(",", verbalizations));
                    return new CompoundDialogueUnit(new List<DialogueUnit> (verbalizations));
                }

                throw new System.Exception("Incorrect input format for \"acceptChangeGoal#->#\" (" + socialMove + ")");

            case "askHowToDo#":
                verbalization = "How do I ";
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactPotentialAction potentialAction) {

                        verbalization += v.VerbalizeAction(potentialAction.action, true);
                    }
                }

                verbalization += "?";
                break;

            case "tellHowToDo#":
                verbalization = "It's not hard. Here, have my recipe.";
                break;

            case "frustratedByGoals":
                //I want X, Y, and Z but I can't figure out how!

                verbalization =  VerbalizeFrustrations(socialMove.mentionedFacts);
                if (verbalization != "") {
                    verbalization = "I want " + verbalization;
                } else {
                    verbalization = "I'm doing just fine!";
                    emotion = NPCPortrait.State.happy;
                }
                break;
            case "askRelationWith":
                verbalization = "What do you think about...";
                break;
            case "askRelationWith#":
                verbalization = "What do you think about " + VerbalizationDictionary.Replace(socialMove.arguements[0])+"?";
                break;
            case "tellRelationWith#":

                pair = VerbalizePeopleOpinion(socialMove.mentionedFacts);
                verbalization = pair.Key;
                if (pair.Value != NPCPortrait.State.none) emotion = pair.Value;
                break;
            case "openTellRelationWith#":
                verbalization = "I think...";
                break;
            case "tell#Relation#":
                facts = socialMove.complexFacts;

                
                Relationship.Tag tag = Relationship.Tag.acquantences;
                if(facts.Count > 0 &&
                    facts[0] is WorldFactRelation factRelation ) {
                    tag = factRelation.relation.tag;
                    subject = factRelation.relation.source;
                }

                subject = VerbalizationDictionary.Replace(subject);

                switch (tag) {
                    case Relationship.Tag.bestFriend:
                        verbalization = subject + " thinks you are best friends.";
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

            case "acceptRelationshipView":
                if (socialMove.complexFacts.Count != 3) throw new System.Exception("Incorrect format");

                
                if(socialMove.complexFacts[0] is WorldFactRelation relationFact) {
                    subject = relationFact.relation.source;
                }else throw new System.Exception("Incorrect format");

                List<Relationship.Tag> tags = new List<Relationship.Tag>(
                            from fact in socialMove.complexFacts
                            where fact is WorldFactRelation relation
                            select ((WorldFactRelation)fact).relation.tag
                        );

                if (tags.Count != 3) throw new System.Exception("Incorrect format");

                Relationship.Tag speakerRomanticTag =
                    townie.townieInformation.relationships.GetStrongestTagOnAxis(subject, Relationship.Axis.romantic);
                Relationship.Tag speakerFriendlyTag =
                    townie.townieInformation.relationships.GetStrongestTagOnAxis(subject, Relationship.Axis.friendly);

                Relationship.Tag friendlyTag = tags[0];
                Relationship.Tag romanticTag = tags[1];
                Relationship.Tag newTag = tags[2];

                if(friendlyTag == newTag || romanticTag == newTag) {
                    verbalization = "I think so too.";
                }

                //if told about someone liking them
                if(Relationship.romanticTags.Contains(newTag)) {
                    //if it is more than they thought
                    if(romanticTag < newTag) {
                        
                        //and they like them
                        if(speakerRomanticTag > Relationship.Tag.no_affection) {
                            verbalization = "Oh, really? You think so?";
                            emotion = NPCPortrait.State.blushing;
                        
                        // or if they are friends
                        } else if(speakerFriendlyTag > Relationship.Tag.acquantences) {
                            verbalization = "Oh. Um... Hm... I don't know how to feel about that.";
                        } else {
                            verbalization = "Oh. They do? Um. I didn't know that.";
                        }

                    //if it is less than they thought
                    } else if(romanticTag > newTag) {

                        //and they like them
                        if (speakerRomanticTag > Relationship.Tag.no_affection) {
                            verbalization = "Oh, you think they like me only that much?";
                            emotion = NPCPortrait.State.sad;

                            // and they don't like them
                        } else {
                            verbalization = "Oh, you think they only ";
                            switch (newTag) {
                                case Relationship.Tag.crushing_on:
                                    verbalization += "have a crush on me?";
                                    break;
                                case Relationship.Tag.in_love_with:
                                    verbalization += "love me?";
                                    break;
                                default:
                                    verbalization += "have a crush on me?";
                                    break;
                            }

                            verbalization += " I was afraid they ";

                            switch (romanticTag) {
                                case Relationship.Tag.crushing_on:
                                    verbalization += "had a crush on me.";
                                    break;
                                case Relationship.Tag.in_love_with:
                                    verbalization += " were in love with me.";
                                    break;
                                case Relationship.Tag.head_over_heels:
                                    verbalization += " were head over heels in love with me.";
                                    break;
                                default:
                                    verbalization += " were head over heels in love with me.";
                                    break;
                            }

                        }

                    } else {
                        verbalization = "You think so too!";
                    }
                }else if (Relationship.friendlyTags.Contains(newTag)) {
                    //if the new tag is positively friendly
                    if (newTag > Relationship.Tag.acquantences) {
                        //if it is more than they thought
                        if (friendlyTag < newTag) {

                            //and we are friends
                            if (speakerFriendlyTag > Relationship.Tag.acquantences) {
                                verbalization = "They really think we are that close? I'm glad!";
                                emotion = NPCPortrait.State.happy;
                            } else {
                                verbalization = "Why do they think that? We aren't friends.";
                                emotion = NPCPortrait.State.angry;
                            }

                        } else if (friendlyTag > newTag) {

                            verbalization = "They don't think we're closer than that?";
                            emotion = NPCPortrait.State.sad;
                        } else {
                            verbalization = "I thought so too.";
                        }

                    //if the new tag is negative
                    } else {
                        //if we are friends
                        if (speakerFriendlyTag > Relationship.Tag.acquantences) {
                            verbalization = "They don't like me?";
                            emotion = NPCPortrait.State.sad;
                        } else {
                            //if the value higher than they thought
                            if (newTag > friendlyTag) {
                                verbalization = "They dislike me only that much huh?";
                            } else if (newTag < friendlyTag) {
                                verbalization = "Hmp. That so?";
                            } else {
                                verbalization = "I thought so too.";
                            }
                        }
                    }
                } else {
                    verbalization = "That so?";
                }

                break;

            case "whyGoalMenu":
                verbalization = "Why do you want...";
                break;
            case "whyActionMenu":
                verbalization = "Why did you do...";
                break;
            case "confirmGoalMenu":
                verbalization = "Do you still want...";
                break;
            case "tellActionMenu":
                verbalization = "Did you hear that... did...";
                break;

            case "tellPreferenceMenu":
                verbalization = "Did you know that... likes...";
                break;
            case "goodbye":
                verbalization = "Goodbye!";
                break;
            case "goodbyeThank":
                verbalization = "Thanks for the meal! It was delicious as always.";
                emotion = NPCPortrait.State.happy;
                break;
            case "goodbyeThankRecipe":
                verbalization = "Thank you so much!";
                emotion = NPCPortrait.State.happy;
                break;
            case "goodbyeDejected":
                verbalization = "Oh, okay. I understand... Thank you for the meal.";
                emotion = NPCPortrait.State.sad;
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

        foreach (WorldFact fact in facts) {

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
        if (opinionOfDish != null)
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

    string VerbalizeFrustrations(List<WorldFact> facts) {
        string verbalization = "";
        List<string> frustrations = new List<string>();
        foreach (WorldFact fact in facts) {
            if (fact is WorldFactGoal factGoal &&
                factGoal.goal is GoalState goalState)
                frustrations.Add(goalState.state.Verbalize(townie.Id, partner, true));
        }

        if (frustrations.Count > 0) {
            verbalization += Verbalizer.MakeNiceList(frustrations);

            verbalization += " but I can't figure out how.";
        }

        return verbalization;
    }


    string TalkAboutPlayerDirectedGoal(CompoundSocialMove socialMove)
    {
        //I did X, Y, and Z. 
        //I still need A, B, C. 
        //But I can't figure out how to B or C. 
        //But I can't figure out how to do any of that.

        string completedActions = "";
        string todoGoals = "";
        string stuckGoals = "";
        foreach(SocialMove move in socialMove.socialMoves) {
            if(move.verb == "listActionsTaken" && move.mentionedFacts.Count > 0) {
                completedActions = VerbalizeAllEvents(move.mentionedFacts);
            }
            if(move.verb == "tellToDo" && move.mentionedFacts.Count > 0) {

                List<string> goals = new List<string>();
                List<string> stuck = new List<string>();
                foreach (WorldFact fact in move.mentionedFacts) {
                    if (fact is WorldFactGoal goalFact) {
                        string goalVerbalization = v.VerbalizeGoal(goalFact.goal);

                        goals.Add(goalVerbalization);

                        if (goalFact.modifier.Contains(WorldFactGoal.Modifier.stuck)) {
                            stuck.Add(goalVerbalization);
                        }
                    }
                }

                if(goals.Count > 0)
                    todoGoals = "I still need " + Verbalizer.MakeNiceList(goals) + ".";

                if (goals.Count == stuck.Count) {
                    stuckGoals = "But I can't figure out how to do any of that.";
                } else if(stuck.Count > 0) {
                    stuckGoals = "But I can't figure out how " + Verbalizer.MakeNiceList(stuck) + ".";
                }
            }
        }

        return completedActions + todoGoals + stuckGoals; 
    }


    string VerbalizeGoalTrees(CompoundSocialMove compoundSocialMove)
    {
        List<string> initialStarts = new List<string>() {
            "I really want #"
        };

        List<string> starts = new List<string>() {
            "Also, I want #",
            "I also want #"
        };
        
        List<string> continuations = new List<string>() {
            ". To do that, #",
            ". For that to happen, #",
            ", so #",
            " and so #"
        };

        List<SocialMove> socialMoves = compoundSocialMove.socialMoves;
        List<string> completeVerbalization = new List<string>() {
                VerbalizeGoalBranch(socialMoves[0],
                                    PickRandom(initialStarts),
                                    PickRandom(continuations))
        };
        socialMoves.RemoveAt(0);

        List<string> alsos = new List<string>() { "Also" };
        foreach(SocialMove move in socialMoves) {

            string start = PickRandom(starts);
            if(start == null) {
                alsos.Add("also");
                start = string.Join(", ", alsos);
                start += ", #";
            }

            string continuation = "";
            if (move.mentionedFacts.Count > 1) {
                continuation = PickRandom(continuations, false);
            }

            string partialVerbalization = VerbalizeGoalBranch(move, start, continuation);

            completeVerbalization.Add(partialVerbalization);

        }

        return string.Join(".\n", completeVerbalization) + ".";
    }

    string PickRandom(List<string> lst, bool removeAfter = true)
    {
        if (lst.Count == 0) return null;

        int index = Random.Range(0, lst.Count);
        string value = lst[index];
        if(removeAfter) lst.RemoveAt(index);

        return value;
    }

    string VerbalizeGoalBranch(SocialMove move, string start, string continuation)
    {
        string verbalization = start;

        List<string> goals = new List<string>();
        foreach (WorldFact fact in move.mentionedFacts) {
            
            if (fact is WorldFactGoal goalFact) {
                goals.Add(v.VerbalizeGoal(goalFact.goal));
            }
        }

        verbalization = verbalization.Replace("#", goals[0]);
        goals.RemoveAt(0);

        if(continuation != "" && goals.Count > 0) {
            verbalization += continuation;
            verbalization = verbalization.Replace("#", "I want "+ Verbalizer.MakeNiceList(goals));
        }

        return verbalization;
    }
}
