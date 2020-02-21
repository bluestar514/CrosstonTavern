using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionExecutionManager : ActionManager
{
    Person actor;
    Registry people;
    Map map;

    public ActionExecutionManager(Person actor, Registry people, Map map)
    {
        this.actor = actor;
        this.people = people;
        this.map = map;
    }



    public ExecutedAction ExecuteAction(ChosenAction chosenAction)
    {
        WeightedAction action = chosenAction.Action;

        Effect chosenEffect = PickEffect(action);

        foreach(MicroEffect effect in chosenEffect.effects) {
            if (effect is InvChange) {
                InvChange invChange = (InvChange)effect;

                Person subject = people.GetPerson(invChange.InvOwner);

                subject.ChangeInventoryContents(invChange.DeltaMax, invChange.ItemId[0]);
            }


            if (effect is Move) {
                Move move = (Move)effect;

                actor.Move(move.TargetLocation);
            }


            if (effect is SocialChange) {
                SocialChange socialChange = (SocialChange)effect;

                Person source = people.GetPerson(socialChange.SourceId);

                source.ChangeRelationshipValue(socialChange.DeltaMin, socialChange.TargetId);
            }
        }

        ExecutedAction finalAction = new ExecutedAction(chosenAction, chosenEffect);

        actor.history.Add(finalAction);
        return finalAction;
    }

    Effect PickEffect(WeightedAction action)
    {
        Dictionary<string, List<string>> resources = GetActionResources(map, action, actor);

        List<Effect> potentialEffects = action.GenerateKnownEffects(resources);
        EvaluateChances(potentialEffects);

        potentialEffects.OrderBy(effect => effect.evaluatedChance);

        float randomNumber = Random.value * MaxChance(potentialEffects);

        foreach (Effect effect in potentialEffects) {
            if (randomNumber < effect.evaluatedChance) return PickSpecificEffect(effect);
            else randomNumber -= effect.evaluatedChance;
        }

        return PickSpecificEffect(potentialEffects.Last());
    }
    Effect PickSpecificEffect(Effect effect)
    {
        List<MicroEffect> microEffects = new List<MicroEffect>();

        foreach(MicroEffect microEffect in effect.effects) {

            microEffects.Add(microEffect.SpecifyEffect());

        }

        return new Effect(effect.chanceModifier, microEffects);
    }

    void EvaluateChances(List<Effect> effects)
    {
        foreach(Effect effect in effects) {
            effect.EvaluateChance();
        }
    }

    float MaxChance(List<Effect> possibleEffects)
    {
        float sum = 0;

        foreach (Effect effect in possibleEffects) {
            sum += effect.evaluatedChance;
        }

        return sum;
    }

}
