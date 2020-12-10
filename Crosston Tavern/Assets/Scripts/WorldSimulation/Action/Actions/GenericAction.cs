using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GenericAction: WorldAction {

    public List<BindingPort> bindingPorts;
    

    public GenericAction(string id, int executionTime, Precondition preconditions, 
        List<Outcome> potentialEffects, List<BindingPort> bindings, VerbilizationAction verbilizationInfo) : 
        base(id, executionTime, preconditions, potentialEffects, verbilizationInfo)
    {
        if (bindings == null) this.bindingPorts = null;
        else this.bindingPorts = new List<BindingPort>(bindings);

        name = ToString();
    }

    public override string ToString()
    {
        return "<"+Id+"(generic)>";
    }

}


