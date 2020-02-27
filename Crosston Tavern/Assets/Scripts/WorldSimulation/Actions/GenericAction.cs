using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericAction: WorldAction {

    public int executionTime;
    public List<Condition> preconditions;
    public List<Effect> potentialEffects;

    public GenericAction(string id, int executionTime, List<Condition> preconditions, List<Effect> potentialEffects):base(id)
    {
        this.executionTime = executionTime;
        this.preconditions = new List<Condition>(preconditions);
        this.potentialEffects = new List<Effect>(potentialEffects);
        name = ToString();
    }

    public override string ToString()
    {
        return "<"+Id+"(generic)>";
    }

    public bool SatisfiedPreconditions(Person actor, Feature feature, Location location)
    {
        return !preconditions.Exists(condition => !condition.InEffect(actor, feature, location));
    }
}

