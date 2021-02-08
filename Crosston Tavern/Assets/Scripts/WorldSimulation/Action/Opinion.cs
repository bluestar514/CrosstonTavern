﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Opinion 
{
    public List<WeightedAction.WeightRational> expectation;
    float expectationWeight;
    public List<WeightedAction.WeightRational> reality;
    float realityWeight;

    Person townie;

    public List<Tag> tags;

    public enum Tag
    {
        excited,
        disapointed,
        surprised,
        aboutPersonOfInterest,
        noteworthy
    }

    public Opinion(ExecutedAction executedAction, Townie townie)
    {
        List<Effect> realizedEffects = executedAction.executedEffect;
        BoundAction boundAction = executedAction.Action;

        Outcome outcome = new Outcome(new ChanceModifier(), realizedEffects, new List<VerbilizationEffect>() { new VerbilizationEffectItemGather("OPINION_SYSTEM")});
        WorldState ws = townie.ws;
        ActionHeuristicManager ahm = new ActionHeuristicManager(townie.townieInformation, ws);
        this.townie = townie.townieInformation;

        SetExpectation(ahm, boundAction);
        SetReality(ahm, outcome);

        SetTags(executedAction, ws);
    }

    public Opinion()
    {
        tags = new List<Tag>();
    }

    void SetExpectation(ActionHeuristicManager ahm, BoundAction action)
    {

        WeightedAction weightedAction = ahm.GetWeightOfBoundAction(action);

        expectation = weightedAction.weightRationals;
        expectationWeight = weightedAction.weight;
    }

    void SetReality(ActionHeuristicManager ahm, Outcome outcome)
    {
        
        KeyValuePair<float, List<WeightedAction.WeightRational>> realizedRational = ahm.GetWeightOfOutcome(outcome,
            new BoundBindingCollection(new List<BoundBindingPort>()), new FeatureResources(new StringStringListDictionary()), townie.knownGoals);

        reality = realizedRational.Value;
        realityWeight = realizedRational.Key;
    }

    void SetTags(ExecutedAction executedAction, WorldState ws)
    {
        tags = new List<Tag>();
        if (realityWeight > expectationWeight) tags.Add(Tag.excited);
        if (realityWeight < expectationWeight) tags.Add(Tag.disapointed);
        if (realityWeight > 0 && expectationWeight == 0) tags.Add(Tag.surprised);


        //MarkIfDoneByPeopleIfInterest(executedAction, ws);
    }

    void MarkIfDoneByPeopleIfInterest(ExecutedAction executedAction, WorldState ws)
    {
        List<string> peopleOfInterest = new List<string>(from goal in townie.knownGoals
                                                         where goal.state is StateRelation
                                                         select ((StateRelation)goal.state).source);
        peopleOfInterest.AddRange(from goal in townie.knownGoals
                                  where goal.state is StateRelation
                                  select ((StateRelation)goal.state).target);

        peopleOfInterest.AddRange(from goal in townie.knownGoals
                                  where goal.state is StateSocial
                                  select ((StateSocial)goal.state).sourceId);
        peopleOfInterest.AddRange(from goal in townie.knownGoals
                                  where goal.state is StateSocial
                                  select ((StateSocial)goal.state).targetId);

        peopleOfInterest = new List<string>(peopleOfInterest.Distinct());

        if (peopleOfInterest.Contains(townie.id)) peopleOfInterest.Remove(townie.id);

        foreach (Feature feature in ws.map.GetAllFeatures()) {
            if (feature.type != Feature.FeatureType.person && peopleOfInterest.Contains(feature.id)) {
                peopleOfInterest.Remove(feature.id);
            }
        }


        if (peopleOfInterest.Contains(executedAction.Action.ActorId) ||
            peopleOfInterest.Contains(executedAction.Action.FeatureId)) {
            tags.Add(Tag.aboutPersonOfInterest);
        }
    }
}
