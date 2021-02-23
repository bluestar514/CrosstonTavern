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

    public string VerbalizeState(State goalState)
    {
        string owner = "";
        string target;

        if (goalState is StateInventoryStatic) {
            StateInventoryStatic stateInv = (StateInventoryStatic)goalState;

            if (stateInv.ownerId != speakerId) {
                owner = stateInv.ownerId + " to ";
            }

            owner = VerbalizationDictionary.Replace(owner);
            string item = VerbalizationDictionary.Replace(stateInv.itemId, false);

            return (owner + "have " + stateInv.min + " to " + stateInv.max + " " + item);
        } else if (goalState is StateSocial) {
            StateSocial stateSocial = (StateSocial)goalState;

            string axisDirection = "";
            if (ws != null) {
                if (stateSocial.max > ws.GetRelationshipsFor(stateSocial.sourceId).Get(stateSocial.sourceId, stateSocial.axis)) axisDirection = " more";
                else axisDirection = " less";
            }
            if (stateSocial.sourceId != speakerId) {
                owner = stateSocial.sourceId;
            }
            if (stateSocial.targetId == speakerId) target = "me";
            else target = stateSocial.targetId;

            owner = VerbalizationDictionary.Replace(owner);
            target = VerbalizationDictionary.Replace(target);

            return (owner + " to be" + axisDirection + " " + stateSocial.axis + " with " + target);
        } else if (goalState is StatePosition) {
            StatePosition statePos = (StatePosition)goalState;

            string location = VerbalizationDictionary.Replace(statePos.locationId);

            return ("go to the " + location);
        } else if (goalState is StateRelation) {
            StateRelation stateRelation = (StateRelation)goalState;

            if (stateRelation.source != speakerId) {
                owner = stateRelation.source + " to ";
            }
            if (stateRelation.target == speakerId) target = "me";
            else target = stateRelation.target;

            owner = VerbalizationDictionary.Replace(owner);
            target = VerbalizationDictionary.Replace(target);

            return (owner + "to be " + stateRelation.tag + " " + target);
        } else {
            return (goalState.id);
        }
    }

    public string VerbalizeAction(BoundAction action, bool presentTense)
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

        string verbilization = action.verbilizationInfo.Verbilize(actionActor, actionLocation, presentTense);
        verbilization = action.Bindings.BindString(verbilization);

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

        verbalization = action.Action.Bindings.BindString(verbalization);

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
            return VerbalizeState(goalState.state);
        }
       if(goal is GoalAction goalAction) {
            return VerbalizeAction(goalAction.action, true);
        }

        return goal.ToString();
    }

    public static string MakeNiceList(List<string> collectedEvents)
    {
         collectedEvents = new List<string>(collectedEvents.Distinct());

        string verbalization = "";

        if (collectedEvents.Count >= 3) {
            string lastevent = collectedEvents.Last();
            collectedEvents.RemoveAt(collectedEvents.Count - 1);

            verbalization += string.Join(", ", collectedEvents) + ", and " + lastevent;
        } else if (collectedEvents.Count == 2) {
            verbalization += string.Join(" and ", collectedEvents);
        } else if (collectedEvents.Count == 1) {
            verbalization += collectedEvents[0];
        } else {
            verbalization = "";
        }

        return verbalization;
    }



}
