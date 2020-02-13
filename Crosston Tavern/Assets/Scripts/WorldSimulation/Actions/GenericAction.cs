using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericAction: WorldAction {

    public List<Condition> preconditions;
    public List<Effect> potentialEffects;

    public GenericAction(string id, List<Condition> preconditions, List<Effect> potentialEffects):base(id)
    { 
        this.preconditions = preconditions;
        this.potentialEffects = potentialEffects;
    }

    public override string ToString()
    {
        return "<"+Id+"(generic)>";
    }
}

