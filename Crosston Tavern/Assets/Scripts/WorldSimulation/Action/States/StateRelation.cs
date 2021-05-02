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

    public override string Verbalize(string speakerId, string listenerId, bool goal, bool futureTense)
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
                case Relationship.Tag.crushing_on:
                    if (source == "I") return "to have a crush on " + target;
                    else return source + " to have a crush on " + target;
                default:
                    switch (this.tag) {
                        case Relationship.Tag.bestFriend:
                            tag = "best friends";
                            break;
                        case Relationship.Tag.friend:
                            tag = "friends";
                            break;
                        case Relationship.Tag.head_over_heels:
                            tag = "deeply in love";
                            break;
                        case Relationship.Tag.in_love_with:
                            tag = "in love";
                            break;
                        case Relationship.Tag.enemy:
                            tag = "enemies";
                            break;
                        case Relationship.Tag.nemisis:
                            tag = "mortal enemies";
                            break;
                    }

                    if (source == "I")
                        return "to be " + tag + " with " + target;
                    else
                        return source + " to be " + tag + " with " + target;
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

                    case Relationship.Tag.crushing_on:
                        return sourceHasTarget(source, "a crush on", target);
                    case Relationship.Tag.in_love_with:
                        return sourceIsTarget(source, "in love with", target);
                    case Relationship.Tag.head_over_heels:
                        return sourceIsTarget(source, "in deeply love with", target);

                    case Relationship.Tag.friend:
                        return sourceIsTarget(source, "friends with", target);
                    case Relationship.Tag.bestFriend:
                        return sourceIsTarget(source, "best friends with", target);

                    default:
                        return source + " is " + tag + "s with " + target;
                }
            }
        }
    }

    string sourceIsTarget(string source, string state, string target)
    {
        string IS = "is";
        if (source == "I") IS = "am";

        return string.Join(" ", new List<string>() { source, IS, state, target });
    }

    string sourceHasTarget(string source, string state, string target)
    {
        string has = "has";
        if (source == "I") has = "have";

        return string.Join(" ", new List<string>() { source, has, state, target });
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
