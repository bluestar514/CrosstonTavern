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

    public string VerbalizaeState(State goalState)
    {
        string owner = "";
        string target;

        if (goalState is StateInventoryStatic) {
            StateInventoryStatic stateInv = (StateInventoryStatic)goalState;

            if (stateInv.ownerId != speakerId) {
                owner = stateInv.ownerId + " to ";
            }
            return (owner + "have " + stateInv.min + " to " + stateInv.max + " " + stateInv.itemId);
        } else if (goalState is StateSocial) {
            StateSocial stateSocial = (StateSocial)goalState;

            string axisDirection = "";
            if (ws != null) {
                if (stateSocial.max > ws.GetRelationshipsFor(stateSocial.sourceId).Get(stateSocial.sourceId, stateSocial.axis)) axisDirection = " nmore";
                else axisDirection = " less";
            }
            if (stateSocial.sourceId != speakerId) {
                owner = stateSocial.sourceId;
            }
            if (stateSocial.targetId == speakerId) target = "me";
            else target = stateSocial.targetId;

            return (owner + "to be" + axisDirection + " " + stateSocial.axis + " with " + target);
        } else if (goalState is StatePosition) {
            StatePosition statePos = (StatePosition)goalState;
            return ("go to the " + statePos.locationId);
        } else if (goalState is StateRelation) {
            StateRelation stateRelation = (StateRelation)goalState;

            if (stateRelation.source != speakerId) {
                owner = stateRelation.source + " to ";
            }
            if (stateRelation.target == speakerId) target = "me";
            else target = stateRelation.target;
            return (owner + "to be " + stateRelation.tag + " " + target);
        } else {
            return (goalState.id);
        }
    }

    //I went fishing at the lake
    //Bob went foriging at the forest
    public string VerbalizeAction(ExecutedAction action, bool presentTense)
    {
        string actionActor = action.Action.ActorId;
        if (actionActor == speakerId) {
            actionActor = "I";
        }
        if (actionActor == listenerId) {
            actionActor = "you";
        }
        string actionLocation = action.Action.FeatureId;


        string verbilization = action.Action.verbilizationInfo.Verbilize(actionActor, actionLocation, presentTense);
        verbilization = action.Action.Bindings.BindString(verbilization);

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

        List<string> verbEffects = new List<string>();
        foreach(Effect effect in action.executedEffect) {
            foreach(VerbilizationEffect verbalizationPattern in action.effectVerbalizationPatterns) {
                string str = verbalizationPattern.Verbilize(actionActor, effect);
                if (str != "") verbEffects.Add(str);
            }
        }
        string verbalization = MakeNiceList(verbEffects);
        if (verbalization != "") return VerbalizeAction(action, presentTense) + " and " + verbalization;
        else return VerbalizeAction(action, presentTense);
    }


    public string MakeNiceList(List<string> collectedEvents)
    {
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
