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

    static string INVITEM = "#inventory_item#";
    static string INVITEMCOUNT = "#inventory_item_count#";

    static string STOCKITEM = "#stock_item#";
    static string STOCKCOST = "#stock_price#";
    static string STOCKCOUNT = "#stock_count#";

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

        allActions = MakeNonGeneric(allActions);

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
            if (ActionNeedsInventoryItemBinding(action)) {
                foreach(string item in actor.inventory.GetItemList()) {
                    for(int num= 0; num <=actor.inventory.GetInventoryCount(item); num++) {
                        string id = action.Id.Replace(INVITEM, item);

                        List<Condition> precondition = RebindPreconditionsItems(action.preconditions, item, num, RebindEffectInventoryItems);
                        List<Outcome> outcomes = RebindPotentialOutcomesItems(action.potentialEffects, item, num, RebindEffectInventoryItems);

                        availableActions.Add(new BoundAction(id, action.executionTime, precondition, outcomes, actor.Id,
                            action.FeatureId, action.LocationId, action.OtherBindings));
                    }
                }
            } else {
                availableActions.Add(action);
            }
        }

        actions = availableActions;
        availableActions = new List<BoundAction>();

        foreach (BoundAction action in actions) {
            if (ActionNeedsStockItemBinding(action)) {
                Feature feature = ws.map.GetFeature(action.FeatureId);

                foreach (KeyValuePair<string, int> itemCost in feature.stockTable) {
                    string item = itemCost.Key;
                    int cost = itemCost.Value;

                    string id = action.Id.Replace(STOCKITEM, item);

                    for (int num = 0; num <= feature.inventory.GetInventoryCount(item); num ++) {
                        List<Condition> precondition = RebindPreconditionsItems(action.preconditions, item, num, RebindEffectStockItems);
                        List<Outcome> outcomes = RebindPotentialOutcomesItems(action.potentialEffects, item, num, RebindEffectStockItems);

                        precondition = RebindPreconditionsItems(precondition, item, cost, RebindEffectStockCost);
                        outcomes = RebindPotentialOutcomesItems(outcomes, item, cost, RebindEffectStockCost);

                        availableActions.Add(new BoundAction(id, action.executionTime, precondition, outcomes, actor.Id,
                            action.FeatureId, action.LocationId, action.OtherBindings));
                    }
                }


            } else {
                availableActions.Add(action);
            }
        }



        return availableActions;
    }

    bool ActionNeedsInventoryItemBinding(BoundAction action)
    {
        return ActionNeedsBinding(action, EffectNeedsInventoryItemBinding);
    }

    bool ActionNeedsStockItemBinding(BoundAction action)
    {
        return ActionNeedsBinding(action, EffectNeedsStockItemBinding);
    }

    bool ActionNeedsBinding(BoundAction action, Func<Effect, bool> EffectNeedsKindOfBinding)
    {
        if (EffectNeedsKindOfBinding is null) {
            throw new ArgumentNullException(nameof(EffectNeedsKindOfBinding));
        }

        foreach (Condition condition in action.preconditions) {
            if (condition is Condition_IsState) {
                Condition_IsState condition_IsState = (Condition_IsState)condition;
                Effect state = condition_IsState.state;

                if (EffectNeedsKindOfBinding(state)) return true;
            }
        }

        foreach (Outcome outcome in action.potentialEffects) {
            foreach (Effect effect in outcome.effects) {
                if (EffectNeedsKindOfBinding(effect)) return true;
            }
        }

        return false;
    }

    bool EffectNeedsInventoryItemBinding(Effect effect)
    {
        if (effect is EffectGenericInv) {
            EffectGenericInv invState = (EffectGenericInv)effect;

            List<string> items = invState.ItemId;
            if (items.Contains(INVITEM)) return true;

            string min = invState.DeltaMin;
            string max = invState.DeltaMax;
            if (min.Contains(INVITEMCOUNT) || max.Contains(INVITEMCOUNT) ) {
                return true;
            }
        }

        return false;
    }

    bool EffectNeedsStockItemBinding(Effect effect)
    {
        if (effect is EffectGenericInv) {
            EffectGenericInv invState = (EffectGenericInv)effect;

            List<string> items = invState.ItemId;
            if (items.Contains(STOCKITEM)) return true;

            string min = invState.DeltaMin;
            string max = invState.DeltaMax;
            if (min.Contains(STOCKCOUNT) || min.Contains(STOCKCOST) ||
                max.Contains(STOCKCOUNT) || max.Contains(STOCKCOST)) {
                return true;
            }
        }

        return false;
    }

    List<Condition> RebindPreconditionsItems(List<Condition> origninalPreconditions, string newItem, int count,
                                            Func<Effect, string, int, Effect> RebindEffectItems)
    {
        List<Condition> preconditions = new List<Condition>();

        foreach(Condition condition in origninalPreconditions) {
            if(condition is Condition_IsState) {
                Condition_IsState condition_IsState = (Condition_IsState)condition;
                preconditions.Add(new Condition_IsState(RebindEffectItems(condition_IsState.state, newItem, count)));
                continue;
            }

            preconditions.Add(condition);
        }

        return preconditions;
    }

    List<Outcome> RebindPotentialOutcomesItems(List<Outcome> originalOutcomes, string item, int count, 
                                                Func<Effect, string, int, Effect> RebindEffectItems)
    {
        List<Outcome> potentialOutcomes = new List<Outcome>();

        foreach(Outcome outcome in originalOutcomes) {
            List<Effect> effects = new List<Effect>();
            foreach (Effect effect in outcome.effects) {
                effects.Add(RebindEffectItems(effect, item, count));
            }
            potentialOutcomes.Add(new Outcome(outcome.chanceModifier, effects));
        }

        return potentialOutcomes;
    }

    Effect RebindEffectInventoryItems(Effect effect, string newItem, int count)
    { 

        if(effect is EffectGenericInv) {
            EffectGenericInv inventoryState = (EffectGenericInv)effect;

            List<string> items = new List<string>();
            foreach(string item in items) {
                if (item == INVITEM ) items.Add(newItem);
                else items.Add(item);
            }

            string min = inventoryState.DeltaMin.Replace(INVITEMCOUNT, count.ToString());
            string max = inventoryState.DeltaMax.Replace(INVITEMCOUNT, count.ToString());

            return new EffectGenericInv(min, max, inventoryState.InvOwner, items);

        }

        return effect;
    }

    Effect RebindEffectStockItems(Effect effect, string newItem, int count)
    {

        if (effect is EffectGenericInv) {
            EffectGenericInv inventoryState = (EffectGenericInv)effect;

            List<string> items = new List<string>();
            foreach (string item in items) {
                if (item == STOCKITEM) items.Add(newItem);
                else items.Add(item);
            }

            string min = inventoryState.DeltaMin.Replace(STOCKCOUNT, count.ToString());
            string max = inventoryState.DeltaMax.Replace(STOCKCOUNT, count.ToString());

            return new EffectGenericInv(min, max, inventoryState.InvOwner, items);

        }

        return effect;
    }

    Effect RebindEffectStockCost(Effect effect, string newItem, int count)
    {

        if (effect is EffectGenericInv) {
            EffectGenericInv inventoryState = (EffectGenericInv)effect;

            string min = inventoryState.DeltaMin.Replace(STOCKCOST, count.ToString());
            string max = inventoryState.DeltaMax.Replace(STOCKCOST, count.ToString());

            return new EffectGenericInv(min, max, inventoryState.InvOwner, inventoryState.ItemId);

        }

        return effect;
    }

    List<BoundAction> MakeNonGeneric(List<BoundAction> actions)
    {
        List<BoundAction> allActions = new List<BoundAction>();

        foreach(BoundAction action in actions) {
            allActions.Add(MakeActionNonGeneric(action));
        }

        return allActions;
    }

    BoundAction MakeActionNonGeneric(BoundAction action)
    {

        List<Condition> preconditions = new List<Condition>();

        foreach (Condition condition in action.preconditions) {
            if (condition is Condition_IsState) {
                Condition_IsState condition_IsState = (Condition_IsState)condition;
                preconditions.Add(new Condition_IsState(MakeEffectNonGeneric(condition_IsState.state)));

            } else preconditions.Add(condition);
        }

        List<Outcome> potentialOutcomes = new List<Outcome>();

        foreach (Outcome outcome in action.potentialEffects) {
            List<Effect> effects = new List<Effect>();
            foreach (Effect effect in outcome.effects) {
                effects.Add(MakeEffectNonGeneric(effect));
            }
            potentialOutcomes.Add(new Outcome(outcome.chanceModifier, effects));
        }

        return new BoundAction(action.Id, action.executionTime, preconditions, potentialOutcomes,
            action.ActorId, action.FeatureId, action.LocationId, action.OtherBindings);
    }


    Effect MakeEffectNonGeneric(Effect effect)
    {
        if (effect is EffectGenericInv) {
            EffectGenericInv inv = (EffectGenericInv)effect;

            return new EffectInvChange(EvalExp(inv.DeltaMin), EvalExp(inv.DeltaMax), inv.InvOwner, inv.ItemId);
        }

        return effect;
    }

    int EvalExp(string exp)
    {
        //largely stolen from https://docs.microsoft.com/en-us/dotnet/api/system.string.split?view=netcore-3.1
        string pattern = @"(\d+)\s+([-+*/])\s+(\d+)";

        foreach (System.Text.RegularExpressions.Match m in
            System.Text.RegularExpressions.Regex.Matches(exp, pattern)) {
            int value1 = int.Parse(m.Groups[1].Value);
            int value2 = int.Parse(m.Groups[3].Value);
            switch (m.Groups[2].Value) {
                case "+":
                    return value1 + value2;
                case "-":
                    return value1 - value2;
                case "*":
                    return value1 * value2;
                case "/":
                    return value1 / value2;
            }
        }
        int output = 0;
        if (int.TryParse(exp, out output)) return output;

        Debug.LogError("Something probably went wrong while parsing: " + exp);
        return 0;
    }
}
