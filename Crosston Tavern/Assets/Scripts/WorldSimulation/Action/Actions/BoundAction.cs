using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoundAction : GenericAction
{
    public string ActorId { get; private set; }
    public string FeatureId { get; private set; }
    public string LocationId { get; private set; }
    public List<BoundBindingPort> Bindings { get; private set; }

    public BoundAction(string id, int executionTime, List<Condition> preconditions, List<Outcome> potentialEffects, 
        string actorId, string featureId, string locationId, List<BoundBindingPort> Bindings) :base(id, executionTime, preconditions, potentialEffects, null)
    {
        this.ActorId = actorId;
        this.FeatureId = featureId;
        this.LocationId = locationId;
        this.Bindings = Bindings;

        name = ToString();
    }

    public BoundAction(GenericAction action, string actorId, string featureId, string locationId, List<BoundBindingPort> bindings): 
        this(action.Id, action.executionTime, action.preconditions, action.potentialEffects, actorId, featureId, locationId, bindings)
    { }

    public override string ToString()
    {
        string n = "<" + Id + "(" + ActorId + ", " + FeatureId + ")>";
        if (Bindings == null) return n;

        foreach(BoundBindingPort binding in Bindings) {
            n = n.Replace("#" + binding.tag + "#", binding.Value);
            if(binding is BoundPortInventoryItem) {
                BoundPortInventoryItem item = (BoundPortInventoryItem)binding;
                n = n.Replace("#" + binding.tag + ".count#", item.itemCount.ToString());
            }
            if(binding is BoundPortStockItem) {
                BoundPortStockItem stock = (BoundPortStockItem)binding;
                n = n.Replace("#" + binding.tag + ".cost#", stock.itemCost.ToString());
            }
        }
        
        return n;
    }


}
