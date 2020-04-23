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

    static string INVITEM = "#inventory_item#";

    public ActionBuilder(WorldState worldState, Person actor)
    {
        ws = worldState;
        this.actor = actor;
    }

    public List<BoundAction> BindActions(string locationId)
    {
        if (locationId == "") locationId = actor.location;

        List<BoundAction> allActions = new List<BoundAction>();
        allActions.AddRange(GatherProvidedActionsForActorAt(locationId));

        allActions = GenerateBoundActionsFromInventory(allActions);

        return allActions;
    }

    public List<BoundAction> BindActions()
    {
        return BindActions(actor.location);
    }


    List<BoundAction> GatherProvidedActionsForActorAt(string locationId)
    {

        List<Feature> nearByFeatures = ws.map.GatherFeaturesAt(locationId);

        List<BoundAction> availableActions = new List<BoundAction>();
        foreach (Feature feature in nearByFeatures) {

            availableActions.AddRange(from action in feature.providedActions
                                      select new BoundAction(action, actor.Id, feature.Id, locationId, null));
        }

        return availableActions;
    }

    List<BoundAction> GenerateBoundActionsFromInventory(List<BoundAction> actions)
    {
        

        List <BoundAction> availableActions = new List<BoundAction>();

        foreach(BoundAction action in actions) {
            if (action.Id.Contains(INVITEM)) {
                foreach(string item in actor.inventory.GetItemList()) {
                    string id = action.Id.Replace(INVITEM, item);

                    List<Condition> precondition = RebindPreconditions(action.preconditions, item);
                    List<Outcome> outcomes = RebindPotentialOutcomes(action.potentialEffects, item);

                    availableActions.Add(new BoundAction(id, action.executionTime, precondition, outcomes, actor.Id, action.FeatureId, action.LocationId, action.OtherBindings));
                }
            } else {
                availableActions.Add(action);
            }
        }

        return availableActions;
    }

    List<Condition> RebindPreconditions(List<Condition> origninalPreconditions, string newItem)
    {
        List<Condition> preconditions = new List<Condition>();

        foreach(Condition condition in origninalPreconditions) {
            if(condition is Condition_IsState) {
                Condition_IsState condition_IsState = (Condition_IsState)condition;
                preconditions.Add(new Condition_IsState(RebindMicroEffect(condition_IsState.state, newItem)));
                continue;
            }

            preconditions.Add(condition);
        }

        return preconditions;
    }

    List<Outcome> RebindPotentialOutcomes(List<Outcome> originalOutcomes, string item)
    {
        List<Outcome> potentialOutcomes = new List<Outcome>();

        foreach(Outcome outcome in originalOutcomes) {
            List<Effect> effects = new List<Effect>();
            foreach (Effect effect in outcome.effects) {
                effects.Add(RebindMicroEffect(effect, item));
            }
            potentialOutcomes.Add(new Outcome(outcome.chanceModifier, effects));
        }

        return potentialOutcomes;
    }

    Effect RebindMicroEffect(Effect effect, string newItem)
    { 

        if(effect is InvChange) {
            InvChange inventoryState = (InvChange)effect;

            List<string> items = new List<string>();
            foreach(string item in items) {
                if (item == INVITEM) items.Add(newItem);
                else items.Add(item);
            }

            return new InvChange(inventoryState.DeltaMin, inventoryState.DeltaMax, inventoryState.InvOwner, items);

        }

        return effect;
    }
}
