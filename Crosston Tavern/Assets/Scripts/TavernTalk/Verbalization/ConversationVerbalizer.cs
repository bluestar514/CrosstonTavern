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

        string verbilization = socialMove.ToString();
        string actionName;
        ExecutedAction actionObj;
        List<WorldFact> facts = socialMove.mentionedFacts;
        List<string> goals = new List<string>();
        WorldFactGoal goalFact;
        switch (socialMove.verb) {
            case "askAboutDayHighlights":
                verbilization = "What did you do today?";
                break;
            case "askAboutObservation":
                verbilization = "Did you see anything interesting today?";
                break;
            case "askAboutExcitement":
                verbilization = "Did anything good happen today?";
                break;
            case "askAboutDisapointment":
                verbilization = "Did anything disapointing happen today?";
                break;
            case "askAboutGoals":
                verbilization = "What have you been trying to do lately?";
                break;
            case "askWhyAction#":
                actionName = socialMove.arguements[0];
                actionObj = townie.ws.knownFacts.GetActionFromName(actionName);

                verbilization = v.VerbalizeAction(actionObj, true);
                verbilization = "Why did " + verbilization + "?";
                break;
            case "tellAction#":
                actionName = socialMove.arguements[0];
                actionObj = townie.ws.knownFacts.GetActionFromName(actionName);

                verbilization = v.VerbalizeAction(actionObj, false);
                verbilization = "Did you hear that " + verbilization + "?";
                break;
            case "tellAboutDayEvents":
            case "tellAboutExcitingEvent":
            case "tellAboutDisapointingEvent":
            case "tellAboutDayObservedEvents":
                float coin = Random.value;
                if(coin > .5)
                    verbilization = "Today, "+ VerbalizeAllEvents(facts);
                else
                    verbilization = VerbalizeByTimePeriod(facts);

                break;
            case "tellAboutGoals":
            case "tellWhyGoal#":

                if (facts.Count == 0) {
                    verbilization = "I just do.";
                    break;
                }

                //I want to have 3 to 1000 trout
                //I want to be friendlier with Alicia
                //I want to be dating Alicia
                //I want Alicia to have 4 to 5 strawberry
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizaeState(goalFact.goal.state));
                    }
                }

                verbilization = string.Join(". I want ", goals);
                verbilization = "I want " + verbilization;
                break;
            case "tellWhyAction#":
                foreach (WorldFact fact in facts) {
                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.AddRange(UnrollGoalChain(goalFact.goal));
                    }
                }

                verbilization = string.Join(", and so I wanted ", goals);
                verbilization = "I wanted " + verbilization;
                break;
            case "askWhyGoal#":
                foreach (WorldFact fact in facts) {

                    if (fact is WorldFactGoal) {
                        goalFact = (WorldFactGoal)fact;
                        goals.Add(v.VerbalizaeState(goalFact.goal.state));
                    }
                }

                verbilization = "Why do you want " + goals[0] + "?";
                break;
            case "askAboutAction#":
                actionName = socialMove.arguements[0];
                actionObj = townie.ws.knownFacts.GetActionFromName(actionName);

                verbilization = v.VerbalizeAction(actionObj, false);
                verbilization = "Could you tell me more about how " + verbilization + "?";
                break;
            case "tellDetailsOfAction#":
                foreach (WorldFact fact in facts) {
                    if (fact is WorldFactEvent) {

                        WorldFactEvent e = (WorldFactEvent)fact;
                        ExecutedAction a = e.action;

                        foreach (Effect effect in a.executedEffect) {
                            verbilization += effect.ToString();
                        }
                    }
                }


                break;
            case "acknowledge":
                verbilization = "No, I hadn't";
                Debug.LogWarning("No checking is done as to whether " + townie + " has actually heard " + string.Join(",",socialMove.mentionedFacts));
                break;
        }

        return new DialogueUnit(verbilization, townie.name, socialMove);
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
