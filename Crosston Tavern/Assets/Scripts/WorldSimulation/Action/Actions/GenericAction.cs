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


    public override bool Equals(object obj)
    {
        if (obj is GenericAction genericAction) {
            return ToString() == genericAction.ToString();
        }else if(obj is BoundAction boundAction) {
            return boundAction.Id == Id;
        }else if(obj is ExecutedAction executedAction) {
            return executedAction.Action.Id == Id;
        }

        return false;
    }
}


