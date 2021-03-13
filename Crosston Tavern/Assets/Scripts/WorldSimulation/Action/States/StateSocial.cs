using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSocial : State
{
    public string sourceId;
    public string targetId;
    public Relationship.Axis axis;
    public int min;
    public int max;

    public StateSocial(string sourceId, string targetId, Relationship.Axis axis, int min, int max)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
        this.axis = axis;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateSocial(" +
            sourceId + "-(" + axis.ToString() + ")->" + targetId
            + ",{" + min + "~" + max + "})>";
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);

        float relValue = ws.map.GetPerson(source).relationships.Get(target, axis);

        return relValue <= max && relValue >= min;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);

        return new StateSocial(source, target, axis, min, max);
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StateSocial)) return new List<State>() { this, state };
        StateSocial stateSoc = (StateSocial)state;

        if (stateSoc.sourceId != sourceId ||
            stateSoc.targetId != targetId ||
            stateSoc.axis != axis) return new List<State>() { this, state };

        int min = Mathf.Max(this.min, stateSoc.min);
        int max = Mathf.Min(this.max, stateSoc.max);

        if (min > max) return new List<State>() { this, state };

        return new List<State>() {
            new StateSocial(sourceId, targetId, axis, min, max)
        };
    }

    public override string Verbalize(string speakerId, string listenerId, bool goal, bool futureTense = false)
    {
        return Verbalize(speakerId, listenerId, goal, null, futureTense);
    }

    public string Verbalize(string speakerId, string listenerId, bool goal, WorldState ws, bool futureTense = false)
    {
        string subject = this.sourceId;
        subject = Verbalizer.VerbalizeSubject(subject, speakerId, listenerId);
        string target = this.targetId;
        target = Verbalizer.VerbalizeSubject(target, speakerId, listenerId);
        if (target == "I") target = "me";

        string axisDirection = "";
        if (ws != null) {
            if (this.max > ws.GetRelationshipsFor(this.sourceId).Get(this.sourceId, this.axis)) 
                axisDirection = "more";
            else 
                axisDirection = "less";
        }


        if (goal) {
            if (subject == "I")
                return " to feel " + axisDirection + " " + axis + " toward " + target;
            else
                return subject + " to feel " + axisDirection + " " + axis + " toward " + target;

        } else {
            if (futureTense) {
                return (subject + " will feel " + axisDirection + " " + axis + " toward " + target);
            } else {
                return (subject + " feels " + axisDirection + " " + axis + " toward " + target);
            }
        }
            
        
            
    }

    public override bool Equals(object obj)
    {
        if (obj is StateSocial state) {
            return sourceId == state.sourceId &&
                    targetId == state.targetId &&
                    axis == state.axis;
        } else return false;
    }

    public override int GetHashCode()
    {
        int hashCode = -164353396;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(sourceId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(targetId);
        hashCode = hashCode * -1521134295 + axis.GetHashCode();
        return hashCode;
    }
}