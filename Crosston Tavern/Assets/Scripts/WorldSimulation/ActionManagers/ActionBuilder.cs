using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Job is to make all possible Bound Actions for a particular person from a given World State
/// </summary>
public class ActionBuilder
{
    WorldState ws;
    Person actor;

    public ActionBuilder(WorldState worldState, Person actor)
    {
        ws = worldState;
        this.actor = actor;
    }

    public List<BoundAction> GetAllActions(string locationId="")
    {
        if (locationId == "") locationId = actor.location;


        List<ActionData> genericActions = GatherProvidedActionsForActorAt(locationId);
        Debug.Log(string.Join(",", genericActions));


        List<BoundAction> allActions = new List<BoundAction>();
        foreach(ActionData data in genericActions) {
            allActions.AddRange(FillOutBindings(data));
        }

        Debug.Log(string.Join(",", allActions));

        return allActions;
    }

   
    public List<ActionData> GatherProvidedActionsForActorAt( string locationId)
    {

        List<Feature> nearByFeatures = ws.map.GatherFeaturesAt(locationId);

        List<ActionData> availableActions = new List<ActionData>();
        foreach (Feature feature in nearByFeatures) {

            availableActions.AddRange(from action in feature.providedActions
                                      select new ActionData(action, feature.id, locationId));
                                      
        }

        return availableActions;
    }

    public List<BoundAction> FillOutBindings(ActionData data)
    {
        Dictionary<string, List<BoundBindingPort>> potentialBindings = new Dictionary<string, List<BoundBindingPort>>();

        //bind entities
        foreach(BindingPort port in data.action.bindingPorts) {
            if(port is BindingPortEntity) {
                BindingPortEntity entity = (BindingPortEntity)port;

                switch (entity.role) {
                    case ActionRole.initiator:
                        potentialBindings.Add(entity.tag, new List<BoundBindingPort>() { new BoundPortEntity(entity.tag, actor.id) });
                        break;
                    case ActionRole.recipient:
                        potentialBindings.Add(entity.tag, new List<BoundBindingPort>() { new BoundPortEntity(entity.tag, data.featureId) });
                        break;
                    case ActionRole.bystander:
                        potentialBindings.Add(entity.tag, new List<BoundBindingPort>(from person in ws.GetBystanders(data.locationId)
                                                                                     select new BoundPortEntity(entity.tag, person)));
                        break;
                    case ActionRole.any:
                        potentialBindings.Add(entity.tag, new List<BoundBindingPort>(from person in ws.registry.GetPeople()
                                                                                     select new BoundPortEntity(entity.tag, person.id)));
                        break;
                    case ActionRole.location_any:
                        potentialBindings.Add(entity.tag, new List<BoundBindingPort>(from location in ws.map.GetNameOfLocations()
                                                                                     select new BoundPortEntity(entity.tag, location)));
                        break;
                    case ActionRole.location_current:
                        potentialBindings.Add(entity.tag, new List<BoundBindingPort>() { new BoundPortEntity(entity.tag, actor.location) });
                        break;
                }
            }
        }

        //combine entity variations
        List<List<BoundBindingPort>> potentialCombinations = new List<List<BoundBindingPort>>() { new List<BoundBindingPort>()};
        foreach(List<BoundBindingPort> potentialBinding in potentialBindings.Values) {
            potentialCombinations = CreatePortCombinations(potentialCombinations, potentialBinding);
        }

        //bind items:
        List<List<BoundBindingPort>> itemBoundCombinations = new List<List<BoundBindingPort>>() {  };
        foreach (List<BoundBindingPort> bindings in potentialCombinations) {

            Dictionary<string, List<BoundBindingPort>> itemCombinations = new Dictionary<string, List<BoundBindingPort>>();
            foreach (BindingPort port in data.action.bindingPorts) {
                if (port is BindingPortInventoryItem) {
                    BindingPortInventoryItem invPort = (BindingPortInventoryItem)port;

                    string ownerId = ((BoundPortEntity)GetPortWithTag(invPort.owner, bindings)).participantId;

                    Inventory inv = ws.registry.GetPerson(ownerId).inventory;

                    itemCombinations.Add(port.tag, new List<BoundBindingPort>(from item in inv.GetItemList()
                                                                              select new BoundPortInventoryItem(port.tag, item, inv.GetInventoryCount(item))));


                } else if (port is BindingPortStockItem) {
                    BindingPortStockItem shopPort = (BindingPortStockItem)port;

                    string shopId = ((BoundPortEntity)GetPortWithTag(shopPort.shopId, bindings)).participantId;

                    StringIntDictionary stockTable = ws.map.GetFeature(shopId).stockTable;
                    Inventory inv = ws.map.GetFeature(shopId).inventory;
                    itemCombinations.Add(port.tag, new List<BoundBindingPort>(from item in inv.GetItemList()
                                                                              select new BoundPortStockItem(port.tag, item,
                                                                                        inv.GetInventoryCount(item), stockTable[item])));

                }

            }
            if (itemCombinations.Count > 0) {
                foreach (List<BoundBindingPort> itemBinding in itemCombinations.Values) {
                    itemBoundCombinations.AddRange(CreatePortCombinations(new List<List<BoundBindingPort>>() { bindings }, itemBinding));
                }
            } else {
                itemBoundCombinations.Add(bindings);
            }

        }

        //itemBoundCombinations = potentialCombinations;

        //make into actions
        List<BoundAction> actions = new List<BoundAction>();
        foreach(List<BoundBindingPort> bindings in itemBoundCombinations) {

            actions.Add(new BoundAction(data.action, actor.id, data.featureId, data.locationId, new BoundBindingCollection( bindings)));
        }

        return actions;
    }


    List<List<BoundBindingPort>> CreatePortCombinations(List<List<BoundBindingPort>> existingPorts, List<BoundBindingPort> newPortBindings)
    {
        List<List<BoundBindingPort>> newCombinations = new List<List<BoundBindingPort>>(); 

        foreach(List < BoundBindingPort> existingCombination in existingPorts) {
            foreach(BoundBindingPort newPort in newPortBindings) {
                List<BoundBindingPort> newCombination = new List<BoundBindingPort>(existingCombination);
                newCombination.Add(newPort);
                newCombinations.Add(newCombination);
            }
        }

        return newCombinations;
    }


    BoundBindingPort GetPortWithTag(string tag, List<BoundBindingPort> ports)
    {
        foreach(BoundBindingPort port in ports) {
            if (port.tag == tag) return port;
        }
        return null;
    }

    public class ActionData
    {
        public GenericAction action;
        public string featureId;
        public string locationId;


        public ActionData(GenericAction action, string featureId, string locationId)
        {
            this.action = action;
            this.featureId = featureId;
            this.locationId = locationId;
        }

        public override string ToString()
        {
            return action.ToString();
        }
    }
}

