using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationController 
{
    public Townie townie;
    List<SocialMove> socialMoves;
    string partner;
    Verbalizer v;


    public ConversationController(Townie patron, List<SocialMove> socialMoves, string partner)
    {
        townie = patron;
        this.socialMoves = socialMoves;
        this.partner = partner;
        v = new Verbalizer(townie.townieInformation.id, partner, townie.ws);
    }


    public SocialMove GiveResponse(SocialMove prompt)
    {

        switch (prompt.verb) {
            case "askAboutGoals":
                return new SocialMove("tellAboutGoals", mentionedGoals: GetGoals());
            case "askAboutDayFull":
                List<ExecutedAction> historyFull = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutDayEvents", mentionedActions: historyFull);
            case "askAboutDayHighlights":
                List<ExecutedAction> history = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutExcitingEvent", mentionedActions: new List<ExecutedAction>(from e in history
                                                                                                           where e.opinion.tags.Count > 0
                                                                                                           select e));
            case "askAboutObservation":
                List<ExecutedAction> observedHistory = FilterOtherActions(GetDayEvents());
                return new SocialMove("tellAboutDayObservedEvents", mentionedActions: observedHistory);
            case "askWhyGoal#":
                string goalName = prompt.arguements[0];
                WorldFactGoal goalFact = (WorldFactGoal)prompt.mentionedFacts[0];
                Goal goal = goalFact.goal;

                List<Goal> parentGoals = goal.GetParentGoals();

                return new SocialMove("tellWhyGoal#", new List<string>() { goalName }, mentionedGoals: parentGoals);
            case "askWhyAction#":
                string actionName = prompt.arguements[0];
                ExecutedAction action = townie.ws.knownFacts.GetActionFromName(actionName);
                if (action == null) return new SocialMove("dontKnow#", prompt.arguements);
                return new SocialMove("tellWhyAction#", new List<string>() { actionName },
                                                mentionedGoals: new List<Goal>(from rational in action.Action.weightRationals
                                                                               select rational.goal));
            case "askAboutExcitement":
                List<ExecutedAction> fullHistory = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutExcitingEvent", mentionedActions: new List<ExecutedAction>(from e in fullHistory
                                                                                                           where e.opinion.tags.Contains(Opinion.Tag.excited)
                                                                                                           select e));
            case "askAboutDisapointment":
                List<ExecutedAction> fullHistory2 = FilterMyActions(GetDayEvents());
                return new SocialMove("tellAboutDisapointingEvent", mentionedActions: new List<ExecutedAction>(from e in fullHistory2
                                                                                                           where e.opinion.tags.Contains(Opinion.Tag.disapointed)
                                                                                                           select e));
            case "tellAction#":
                return new SocialMove("acknowledge");
            case "askAboutAction#":
                return new SocialMove("tellDetailsOfAction#", arguements: prompt.arguements, mentionedFacts: prompt.mentionedFacts);
            default:
                return new SocialMove("DEFAULT");
        }
    }

    public List<SocialMove> GetSocialMoves()
    {
        List<SocialMove> moves = new List<SocialMove>(socialMoves);
        moves.AddRange(GenAskWhyGoal());
        moves.AddRange(GenAskWhyAction());
        moves.AddRange(GenTellAction());
        //moves.AddRange(GenAskAboutAction());
        return moves;
    }


    public void LearnFromInput(SocialMove prompt)
    {
        foreach(WorldFact fact in prompt.mentionedFacts) {
            townie.ws.LearnFact(fact);
        }
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
                verbilization = "Did you hear that " + verbilization +"?";
                break;
            case "tellAboutDayEvents":
            case "tellAboutExcitingEvent":
            case "tellAboutDisapointingEvent":
            case "tellAboutDayObservedEvents":
                
                verbilization = "Today, ";

                List<string> collectedEvents = new List<string>();
                foreach(WorldFact fact in facts) {
                    if (fact is WorldFactEvent) {

                        WorldFactEvent e = (WorldFactEvent)fact;
                        ExecutedAction a = e.action;
                        collectedEvents.Add(v.VerbalizeActionWithResults(a, false));
                    }
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
                    verbilization = "Nothing interesting happened today.";
                }
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
                foreach(WorldFact fact in facts) {
                    if(fact is WorldFactGoal) {
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

                verbilization = "Why do you want "+ goals[0]+"?";
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
        }

        return new DialogueUnit(verbilization, townie.name, socialMove);
    }

    List<string> UnrollGoalChain(Goal initGoal)
    {
        List<Goal> goalChain =new List<Goal>() { initGoal } ;

        int x = 0;
        List<Goal> backlog = new List<Goal>() { initGoal };
         
        while (backlog.Count > 0) {
            Goal goal = backlog[0];
            backlog.RemoveAt(0);

            goalChain.AddRange(goal.GetParentGoals());
            backlog.AddRange(goal.GetParentGoals());
            Debug.Log("goal: " + goal + " parents: " + goal.GetParentGoals().Count);
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
        foreach(Goal goal in noDups) {
            subVerbilizations.Add(v.VerbalizaeState(goal.state));
        }

        return subVerbilizations;
        
    }

    

    List<SocialMove> GenAskWhyGoal()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactGoal
                                    select new SocialMove("askWhyGoal#", new List<string> { ((WorldFactGoal)fact).goal.name}, 
                                                                        mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenAskWhyAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == partner //I may take out this condition in the long run
                                    select new SocialMove("askWhyAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                           mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenTellAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId != partner
                                    select new SocialMove("tellAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() }, 
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<SocialMove> GenAskAboutAction()
    {
        return new List<SocialMove>(from fact in townie.ws.knownFacts.GetFacts()
                                    where fact is WorldFactEvent
                                    where ((WorldFactEvent)fact).action.Action.ActorId == partner
                                    select new SocialMove("askAboutAction#", new List<string> { ((WorldFactEvent)fact).action.ToString() },
                                                                         mentionedFacts: new List<WorldFact>() { fact }));
    }

    List<Goal> GetGoals()
    {
        return townie.gm.GetGoalsList();
    }

    List<ExecutedAction> GetDayEvents()
    {
        List<ExecutedAction> completeHistory = townie.ws.knownFacts.GetHistory();
        List<ExecutedAction> todaysHistory = completeHistory.FindAll(x => x.executionTime.SameDay(townie.ws.Time));

        return todaysHistory;
    }

    List<ExecutedAction> FilterMyActions(List<ExecutedAction> todaysHistory)
    {
        List<ExecutedAction> filteredHistory = todaysHistory.FindAll(x => x.Action.ActorId == townie.townieInformation.id ||
                                                                            x.Action.FeatureId == townie.townieInformation.id);

        return filteredHistory;
    }

    List<ExecutedAction> FilterOtherActions(List<ExecutedAction> observedActions)
    {
        List<ExecutedAction> filteredHistory = observedActions.FindAll(x => x.Action.ActorId != townie.townieInformation.id &&
                                                                            x.Action.FeatureId != townie.townieInformation.id);

        return filteredHistory;
    }

}
