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

    public List<WorldFact> AddHistory(ExecutedAction action, WorldState ws)
    {
        history.Add(action);

        List<WorldFact> learnedFacts = new List<WorldFact>();

        foreach(Effect effect in action.executedEffect) {
            if(effect is EffectKnowledge) {
                EffectKnowledge effectKnow = (EffectKnowledge)effect;
                learnedFacts.AddRange(AddFact(effectKnow.fact, ws));
            }
        }

        return learnedFacts;
    }

    public List<WorldFact> AddFact(WorldFact fact, WorldState ws)
    {
        if (!KnowFact(fact)) {
            knownFacts.Add(fact);
            
            return fact.UpdateWorldState(ws);
        }

        return new List<WorldFact>();
    }


    public bool KnowFact(WorldFact fact)
    {
        foreach(WorldFact f in knownFacts) {
            if (f.Equals(fact)) return true;
        }

        return false;
    }

    public List<ExecutedAction> GetHistory()
    {
        return new List<ExecutedAction>(history);
    }

    public List<WorldFact> GetFacts()
    {
        return new List<WorldFact>(knownFacts);
    }

    public ExecutedAction GetActionFromName(string name)
    {
        foreach(ExecutedAction action in history) {
            if (action.ToString() == name) return action;
        }

        return null;
    }
}
