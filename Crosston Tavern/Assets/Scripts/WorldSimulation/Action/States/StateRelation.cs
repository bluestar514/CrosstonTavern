using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRelation : State
{
    public string source;
    public string target;
    public Relationship.Tag tag;

    public StateRelation(string source, string target, Relationship.Tag tag)
    {
        this.source = source;
        this.target = target;
        this.tag = tag;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(this.source);
        string target = bindings.BindString(this.target);

        return new StateRelation(source, target, tag);
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        StateRelation state = (StateRelation)Bind(bindings, resources);

        Person source = ws.map.GetPerson(state.source);

        return source.relationships.RelationTagged(state.target, tag);
    }

    public override string ToString()
    {
        return "<StateRelation(" +
            source + "," + target
            + "," + tag + ")>"; 
    }

    public override string Verbalize(string speakerId, string listenerId, bool goal, bool futureTense = false)
    {
        

        string source = this.source;
        string target = this.target;

        source = Verbalizer.VerbalizeSubject(source, speakerId, listenerId);
        target = Verbalizer.VerbalizeSubject(target, speakerId, listenerId);
        if (target == "I") target = "me";

        string tag = this.tag.ToString();

        if (goal) {
            switch (this.tag) {
                case Relationship.Tag.dating:
                    if (source == "I") return "to date " + target;

                    return source + " to date " + target;
                case Relationship.Tag.liked:
                    if (source == "I")
                        return "to like " + target + " some";
                    else
                        return source + " will like " + target + " some";
                case Relationship.Tag.disliked:
                    if (source == "I")
                        return "to dislike " + target + " some";
                    else
                        return source + " to dislike " + target + " some";
                default:
                    if (source == "I")
                        return "to be " + tag + "s with " + target;
                    else
                        return source + " to be " + tag + "s with " + target;
            }

        } else {
            if (futureTense) {
                switch (this.tag) {
                    case Relationship.Tag.dating:
                        if (source == "I") return "I can date " + target;

                        return source + " will date " + target;
                    case Relationship.Tag.liked:
                        return source + " will like " + target + " some";
                    case Relationship.Tag.disliked:
                        return source + " will dislike " + target + " some";
                    default:
                        return source + " will be " + tag + "s with " + target;
                }
            } else {
                switch (this.tag) {
                    case Relationship.Tag.dating:
                        if (source == "I") return "I am dating " + target;

                        return source + " is dating " + target;
                    case Relationship.Tag.liked:
                        return source + " likes " + target + " some";
                    case Relationship.Tag.disliked:
                        return source + " dislikes " + target + " some";
                    default:
                        return source + " is " + tag + "s with " + target;
                }
            }
        }
    }

    public override List<State> Combine(State state)
    {
        if (state is StateRelation relation &&
            this.Equals(relation)) {
            return new List<State>() { this };
        } else
            return new List<State>() { this, state };
    }

    public override bool Equals(object obj)
    {
        if (obj is StateRelation state) {
            return source == state.source &&
                    target == state.target &&
                    tag == state.tag;
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hashCode = 243036089;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(source);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(target);
        hashCode = hashCode * -1521134295 + tag.GetHashCode();
        return hashCode;
    }
}
