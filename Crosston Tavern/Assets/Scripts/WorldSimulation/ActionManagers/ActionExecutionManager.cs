using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Job is to determine the exact outcome of a Chosen Action.
/// Generates a Executed Action
/// </summary>

public class ActionExecutionManager : ActionManager
{
    Person actor;
    WorldState ws;

    public ActionExecutionManager(Person actor, WorldState ws)
    {
        this.actor = actor;
        this.ws = ws;
    }


    public ExecutedAction ExecuteAction(ChosenAction chosenAction)
    {

        Outcome chosenOutcome = ChooseOutcome(chosenAction.Action);

        List<Effect> realizedEffects = RealizeEffectsOfOutcome(chosenOutcome, chosenAction.Action.Bindings, 
                                                ws.map.GetFeature(chosenAction.Action.FeatureId).relevantResources);

        Outcome realizedOutcome = new Outcome(new ChanceModifierSimple(1), realizedEffects);

        return new ExecutedAction(chosenAction, realizedOutcome);

    }

    Outcome ChooseOutcome(WeightedAction action)
    {
        float totalChance = action.potentialEffects.Sum(outcome => outcome.chanceModifier.Chance(ws));
        float rand = UnityEngine.Random.value * totalChance;

        foreach(Outcome outcome in action.potentialEffects) {
            float num = outcome.chanceModifier.Chance(ws);
            if (num >= rand) return outcome;
            rand -= num;
        }

        throw new System.Exception("Something was not programed correctly, we should not be making it to this line of code");
    }

    List<Effect> RealizeEffectsOfOutcome(Outcome chosenOutcome, BoundBindingCollection bindings, FeatureResources resources)
    {
        List<Effect> realizedEffects = new List<Effect>();

        foreach(Effect effect in chosenOutcome.effects) {
            if(effect is EffectInventory) {
                EffectInventory effectInv = (EffectInventory)effect;

                string owner = bindings.BindString(effectInv.ownerId);
                string itemid = bindings.BindString(effectInv.itemId);

                List<string> items = resources.BindString(itemid);
                itemid = items[Mathf.FloorToInt(UnityEngine.Random.value * items.Count)];

                int delta = effectInv.delta;

                realizedEffects.Add(new EffectInventoryStatic(owner, itemid, delta));


                Inventory inv = ws.GetInventory(owner);
                inv.ChangeInventoryContents(delta, itemid);
            } else
            if(effect is EffectSocial) {
                EffectSocial effectSoc = (EffectSocial)effect;

                string sourceId = bindings.BindString(effectSoc.sourceId);
                string targetId = bindings.BindString(effectSoc.targetId);
                Relationship.RelationType axis = effectSoc.axis;
                int delta = effectSoc.delta;

                realizedEffects.Add(new EffectSocialStatic(sourceId, targetId, axis, delta));


                Relationship rel = ws.GetRelationshipsFor(sourceId);
                rel.Increase(targetId, axis, delta);
            } else
            if(effect is EffectMovement) {
                EffectMovement effectMove = (EffectMovement)effect;

                string moverId = bindings.BindString(effectMove.moverId);
                string newLocationId = bindings.BindString(effectMove.newLocationId);

                List<string> potentialIds = resources.BindString(newLocationId);
                newLocationId = potentialIds[Mathf.FloorToInt(UnityEngine.Random.value * potentialIds.Count)];

                realizedEffects.Add(new EffectMovement(moverId, newLocationId));

                ws.map.MovePerson(moverId, newLocationId);
            } else {
                realizedEffects.Add(effect);

                Debug.LogWarning("Effect ("+effect+") of unaccounted for Effect Type failed to be executed!");
            }
        }

        return realizedEffects;
    }
}
