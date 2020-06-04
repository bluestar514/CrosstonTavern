using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class KnownFacts 
{
    string ownerId;

    [SerializeField]
    List<ExecutedAction> history;
    [SerializeField]
    List<WorldFact> knownFacts;

    public KnownFacts(string ownerId)
    {
        history = new List<ExecutedAction>();
        knownFacts = new List<WorldFact>();
        this.ownerId = ownerId;
    }

    public void AddHistory(ExecutedAction action, WorldState ws)
    {
        history.Add(action);

        foreach(Effect effect in action.executedEffect.effects) {
            if(effect is EffectKnowledge) {
                EffectKnowledge effectKnow = (EffectKnowledge)effect;
                AddFact(effectKnow.fact, ws);
            }
        }
    }

    public void AddFact(WorldFact fact, WorldState ws)
    {
        if (!KnowFact(fact)) {
            knownFacts.Add(fact);
            UpdateResourcesOnMap(fact, ws);
        }
    }


    void UpdateResourcesOnMap(WorldFact fact, WorldState ws)
    {
        string featureId = fact.featureId;
        Feature feature = ws.map.GetFeature(featureId);

        feature.relevantResources.Add(fact.resourceId, fact.potentialBinding);
    }


    public bool KnowFact(WorldFact fact)
    {
        foreach(WorldFact f in knownFacts) {
            if (f == fact) return true;
        }

        return false;
    }
}
