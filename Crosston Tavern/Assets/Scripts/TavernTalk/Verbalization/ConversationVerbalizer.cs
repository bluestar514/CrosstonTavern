using System.Collections;
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
        string actionName;
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
                } else if (state == EntityStatusEffectType.sad.ToString()) {
                    verbalization = "Nothing went right today...";
                } else if (state == EntityStatusEffectType.happy.ToString()) {
                    verbalization = "I had a great day!";
                } else {
                    verbalization = "I'm doing alright. You know how it is.";
                }
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
                verbalization = "The " + socialMove.arguements[0] + " is quite good.";
                break;
            case "order#OnRecomendation":
                verbalization = "Sound good, let's go with that tonight.";
                break;
            case "order#OffRecomendation":
                verbalization = "Mmm, no. I don't think I'm feeling that tonight. How about " +
                                    socialMove.arguements[0] + " instead?";
                break;
            case "order#":
                verbalization = "Can I get a " + socialMove.arguements[0] + "?";
                break;
            case "serveOrder#":
                verbalization = "Coming right up.";
                break;
            case "thank":
                //Thanks. This is good. 
                //Thanks. Oh man, I love strawberry cake. Actually, I like anything with strawberries. 
                //Thanks. This was a good choice, I like anything with strawberries.

                List<string> favorites = new List<string>();
                foreach(WorldFact fact in socialMove.mentionedFacts) {
                    if(fact is WorldFactPreference) {
                        WorldFactPreference preference = (WorldFactPreference)fact;
                        favorites.Add("I " + preference.level + " anything with " + preference.item); //preference.Verbalize(townie.Id, partner));
                    }
                }
                string dishOpinion = "";
                if (socialMove.mentionedFacts.Count > 0)
                    dishOpinion = socialMove.mentionedFacts[0].Verbalize(townie.Id, partner);

                verbalization = "Thanks.";
                if(socialMove.arguements[0] == PreferenceLevel.loved.ToString()) {
                    verbalization += "Oh man, " + dishOpinion + ".";
                    favorites.RemoveAt(0); 
                }else if (socialMove.arguements[0] == PreferenceLevel.liked.ToString()) {
                    verbalization += dishOpinion + ".";
                    favorites.RemoveAt(0);
                }
                else if(socialMove.arguements[0] == PreferenceLevel.neutral.ToString()) {
                    verbalization += "This was a good choice.";
                }else if (socialMove.arguements[0] == PreferenceLevel.disliked.ToString()) {

                } else if (socialMove.arguements[0] == PreferenceLevel.hated.ToString()) {

                }

                if (favorites.Count > 0)
                    verbalization += "Actually, " + Verbalizer.MakeNiceList(favorites);

                break;

            case "askAboutDayHighlights":
                verbalization = "What did you do today?";
                break;
            case "askAboutObservation":
                verbalization = "Did you see anything interesting today?";
                break;
            case "askAboutExcitement":
                verbalization = "Did anything good happen today?";
                break;
            case "askAboutDisapointment":
                verbalization = "Did anything disapointing happen today?";
                break;
            case "askAboutGoals":
                verbalization = "What have you been trying to do lately?";
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

                break;
            case "tellAboutGoals":
            case "tellWhyGoal#":

                if (facts.Count == 0) {
                    verbalization = "I just do.";
                    break;
                }

                List<string> motivations = new List<string>();

                //I want to have 3 to 1000 trout
                //I want to be friendlier with Alicia
                //I want to be dating Alicia
                //I want Alicia to have 4 to 5 strawberry
                foreach (WorldFact fact in facts) {
                    if(fact is WorldFactPotentialAction) {
                        actionFact = (WorldFactPotentialAction)fact;
                        motivations.Add(v.VerbalizeAction(actionFact.action, true));
                    }
                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizaeState(goalFact.goal.state));
                    }
                }
                verbalization = "";
                if (motivations.Count > 0) {
                    verbalization = "I want to " + Verbalizer.MakeNiceList(motivations);
                    verbalization += ", so I can " + Verbalizer.MakeNiceList(goals)+".";
                } else {
                    verbalization = verbalization + "I want " + string.Join(". I want ", goals) + ". ";
                }
                break;
            case "tellWhyAction#":
                foreach (WorldFact fact in facts) {
                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.AddRange(UnrollGoalChain(goalFact.goal));
                    }
                }

                verbalization = string.Join(", and so I wanted ", goals);
                verbalization = "I wanted " + verbalization;
                break;
            case "askWhyGoal#":
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizaeState(goalFact.goal.state));
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
            case "acknowledge":
                verbalization = "No, I hadn't";
                Debug.LogWarning("No checking is done as to whether " + townie + " has actually heard " + string.Join(",",socialMove.mentionedFacts));
                break;
        }

        return new DialogueUnit(verbalization, townie.name, socialMove, NPCPortrait.State.neutral);
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
            subVerbilizations.Add(v.VerbalizaeState(goal.state));
        }

        return subVerbilizations;

    }
}
