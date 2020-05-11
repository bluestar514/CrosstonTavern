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
    WorldState WS;

    public ActionExecutionManager(Person actor, WorldState ws)
    {
        this.actor = actor;
        WS = ws;
    }


    public ExecutedAction ExecuteAction(ChosenAction chosenAction)
    {
        //WeightedAction action = chosenAction.Action;
        //Feature feature = this.map.GetFeature(action.FeatureId);
        //if (!chosenAction.Started()) {
        //    feature.Use();
        //}

        //if (chosenAction.Complete()) {
        //    feature.FinishUse();
        //    Debug.Log(chosenAction);
        //    Outcome chosenEffect = PickEffect(action);

        //    foreach (Effect effect in chosenEffect.effects) {
        //        if (effect is EffectInvChange) {
        //            EffectInvChange invChange = (EffectInvChange)effect;

        //            Inventory inventory = WS.GetInventory(invChange.InvOwner);
        //            inventory.ChangeInventoryContents(invChange.DeltaMax, invChange.ItemId[0]);
        //        }


        //        if (effect is EffectMove) {
        //            EffectMove move = (EffectMove)effect;

        //            actor.Move(move.TargetLocation);
        //        }


        //        if (effect is EffectSocialChange) {
        //            EffectSocialChange socialChange = (EffectSocialChange)effect;

        //            Person source = people.GetPerson(socialChange.SourceId.Replace("person_", ""));

        //            source.relationships.Increase(socialChange.TargetId.Replace("person_", ""), socialChange.RelationType, socialChange.DeltaMax);
        //        }
        //    }

        //    ExecutedAction finalAction = new ExecutedAction(chosenAction, chosenEffect);

        //    actor.history.Add(finalAction);
        //    return finalAction;
        //}

        //chosenAction.Progress();

        return null;
    }

    //Outcome PickEffect(WeightedAction action)
    //{
    //    List<Outcome> potentialEffects = action.GenerateExpectedEffects(WS);
    //    EvaluateChances(potentialEffects);

    //    potentialEffects.OrderBy(effect => effect.evaluatedChance);

    //    float randomNumber = Random.value * MaxChance(potentialEffects);

    //    foreach (Outcome effect in potentialEffects) {
    //        if (randomNumber < effect.evaluatedChance) return PickSpecificEffect(effect);
    //        else randomNumber -= effect.evaluatedChance;
    //    }

    //    return PickSpecificEffect(potentialEffects.Last());
    //}
    //Outcome PickSpecificEffect(Outcome effect)
    //{
    //    List<Effect> microEffects = new List<Effect>();

    //    foreach(Effect microEffect in effect.effects) {

    //        microEffects.Add(microEffect.SpecifyEffect());

    //    }

    //    return new Outcome(effect.chanceModifier, microEffects);
    //}

    //void EvaluateChances(List<Outcome> effects)
    //{
    //    foreach(Outcome effect in effects) {
    //        effect.EvaluateChance(WS);
    //    }
    //}

    //float MaxChance(List<Outcome> possibleEffects)
    //{
    //    float sum = 0;

    //    foreach (Outcome effect in possibleEffects) {
    //        sum += effect.evaluatedChance;
    //    }

    //    return sum;
    //}

}
