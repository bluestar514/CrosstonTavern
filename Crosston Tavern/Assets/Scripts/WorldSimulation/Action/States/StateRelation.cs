using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRelation : State
{
    public string source;
    public string target;
    public Relationship.RelationshipTag tag;

    public StateRelation(string source, string target, Relationship.RelationshipTag tag)
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

    public override string Verbalize(string speakerId, string listenerId, bool goal)
    {
        

        string source = this.source;
        string target = this.target;

        source = Verbalizer.VerbalizeSubject(source, speakerId, listenerId);
        target = Verbalizer.VerbalizeSubject(target, speakerId, listenerId);
        if (target == "I") target = "me";

        string tag = this.tag.ToString();

        if (goal) {
            switch (this.tag) {
                case Relationship.RelationshipTag.dating:
                    if (source == "I") return "to date " + target;

                    return source + " to date " + target;
                case Relationship.RelationshipTag.liked:
                    if (source == "I")
                        return "to like " + target + " some";
                    else
                        return source + " will like " + target + " some";
                case Relationship.RelationshipTag.disliked:
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
            switch (this.tag) {
                case Relationship.RelationshipTag.dating:
                    if (source == "I") return "I can date " + target;

                    return source + " will date " + target;
                case Relationship.RelationshipTag.liked:
                    return source + " will like " + target + " some";
                case Relationship.RelationshipTag.disliked:
                    return source + " will dislike " + target + " some";
                default:
                    return source + " will be " + tag + "s with " + target;
            }
        }
    }
}
