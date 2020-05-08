using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GenericAction: WorldAction {

    public int executionTime;
    public List<Condition> preconditions;
    public List<Outcome> potentialEffects;
    public List<BindingPort> bindings;

    public GenericAction(string id, int executionTime, List<Condition> preconditions, List<Outcome> potentialEffects, List<BindingPort> bindings):base(id)
    {
        this.executionTime = executionTime;
        this.preconditions = new List<Condition>(preconditions);
        this.potentialEffects = new List<Outcome>(potentialEffects);

        if (bindings == null) this.bindings = null;
        else this.bindings = new List<BindingPort>(bindings);

        name = ToString();
    }

    public override string ToString()
    {
        return "<"+Id+"(generic)>";
    }

}

