using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Verbalizer 
{
    string speakerId;
    string listenerId;
    WorldState ws; 

    public Verbalizer(string speakerId, string listenerId, WorldState ws= null)
    {
        this.speakerId = speakerId;
        this.listenerId = listenerId;
        this.ws = ws;
    }

    public string VerbalizeState(State goalState, bool includeSubject = true)
    {

        if (goalState is StateSocial stateSocial) {
            return stateSocial.Verbalize(speakerId, listenerId, false, ws);
        
        } else {
            return  goalState.Verbalize(speakerId, listenerId, false);
        }
    }

    public string VerbalizeAction(BoundAction action, bool presentTense, bool includeSubject = true)
    {
        string actionActor = action.ActorId;
        if (actionActor == speakerId) {
            actionActor = "I";
        }
        if (actionActor == listenerId) {
            actionActor = "you";
        }
        string actionLocation = action.FeatureId;
        if (actionLocation == speakerId) {
            actionLocation = "me";
        }
        if (actionLocation == listenerId) {
            actionLocation = "you";
        }

        actionActor = VerbalizationDictionary.Replace(actionActor);
        actionLocation = VerbalizationDictionary.Replace(actionLocation);

        string verbilization = action.verbilizationInfo.Verbilize(actionActor, actionLocation, presentTense, includeSubject);


        BoundBindingCollection bindings = action.Bindings;
        List<BoundBindingPort> newBindings = new List<BoundBindingPort>();
        foreach (BoundBindingPort bindingPort in bindings.bindings) {
            if (bindingPort.Value == speakerId) {
                newBindings.Add(new BoundBindingPort(bindingPort.tag, "I"));
            } else if (bindingPort.Value == listenerId) {
                newBindings.Add(new BoundBindingPort(bindingPort.tag, "you"));
            } else {
                newBindings.Add(new BoundBindingPort(bindingPort.tag,
                    VerbalizationDictionary.Replace(bindingPort.Value)));
            }
        }

        BoundBindingCollection newCollection = new BoundBindingCollection(newBindings);

        verbilization = newCollection.BindString(verbilization);

        return verbilization;
    }

    //I went fishing at the lake
    //Bob went foriging at the forest
    public string VerbalizeAction(ExecutedAction action, bool presentTense)
    {
        string verbilization = VerbalizeAction(action.Action, presentTense);

        return verbilization;
    }

    // I went fishing at the lake and caught 4 trout.
    public string VerbalizeActionWithResults(ExecutedAction action, bool presentTense)
    {
        

        string actionActor = action.Action.ActorId;
        if (actionActor == speakerId) {
            actionActor = "I";
        }
        if (actionActor == listenerId) {
            actionActor = "you";
        }
        actionActor = VerbalizationDictionary.Replace(actionActor);



        List<string> verbEffects = new List<string>();
        foreach(Effect effect in action.executedEffect) {
            if (effect.verbalization != null) {
                string str = effect.verbalization.Verbilize(actionActor, effect);
                if (str != "") {
                    verbEffects.Add(str);
                }
            }
            
        }
        string verbalization = MakeNiceList(verbEffects);
        if (verbalization != "") verbalization = VerbalizeAction(action, presentTense) + " and " + verbalization;
        else verbalization = VerbalizeAction(action, presentTense);

        BoundBindingCollection bindings = action.Action.Bindings;
        List<BoundBindingPort> newBindings = new List<BoundBindingPort>();
        foreach(BoundBindingPort bindingPort in bindings.bindings) {
            if(bindingPort.Value == speakerId) {
                newBindings.Add(new BoundBindingPort(bindingPort.tag, "I"));
            }else if(bindingPort.Value == listenerId) {
                newBindings.Add(new BoundBindingPort(bindingPort.tag, "you"));
            } else {
                newBindings.Add(new BoundBindingPort(bindingPort.tag,
                    VerbalizationDictionary.Replace(bindingPort.Value)));
            }
        }

        BoundBindingCollection newCollection = new BoundBindingCollection(newBindings);

        verbalization = newCollection.BindString(verbalization);

        return verbalization;
    }

     public string VerbalizeActionWithDate(ExecutedAction action, bool presentTense)
    {
        string verbalization = VerbalizeAction(action, presentTense);
        WorldTime date = action.executionTime;

        if(ws != null)
            verbalization += " " + DateDifferenceToWords(ws.Time, date);

        return verbalization;
    }

    public static string DateDifferenceToWords(WorldTime today, WorldTime date)
    {
        WorldTime.CasualTimeBlocks block = today.GetGeneralDifference(date);

        switch (block) {
            case WorldTime.CasualTimeBlocks.today:
                return "today";
            case WorldTime.CasualTimeBlocks.yesterday:
                return "yesterday";
            case WorldTime.CasualTimeBlocks.days:
            default:
                return (today.ConvertToDayCount() - date.ConvertToDayCount()).ToString() + " days ago";
        }

    }

    public string VerbalizeGoal(Goal goal)
    {
       if(goal is GoalState goalState) {
            if (goalState.state is StateSocial stateSocial)
                return stateSocial.Verbalize(speakerId, listenerId, true, ws);

            return goalState.state.Verbalize(speakerId, listenerId, true);
        }
       if(goal is GoalAction goalAction) {
            return VerbalizeAction(goalAction.action, true, goalAction.action.ActorId != speakerId);
        }

        return goal.ToString();
    }

    public static string MakeNiceList(List<string> collectedEvents, bool and = true)
    {
         collectedEvents = new List<string>(collectedEvents.Distinct());

        string verbalization = "";

        string AND = "and";
        if (!and) AND = "or";

        if (collectedEvents.Count >= 3) {
            string lastevent = collectedEvents.Last();
            collectedEvents.RemoveAt(collectedEvents.Count - 1);

            verbalization += string.Join(", ", collectedEvents) + ", "+AND+" " + lastevent;
        } else if (collectedEvents.Count == 2) {
            verbalization += string.Join(", " + AND + " ", collectedEvents);
        } else if (collectedEvents.Count == 1) {
            verbalization += collectedEvents[0];
        } else {
            verbalization = "";
        }

        return verbalization;
    }

    static public string VerbalizeSubject(string subjectId, string speakerId, string listenerId)
    {
        string name = subjectId;
        
        if (name == speakerId) name = "I";
        if (name == listenerId) name = "you";
        name = VerbalizationDictionary.Replace(name);

        return name;
    }



}
