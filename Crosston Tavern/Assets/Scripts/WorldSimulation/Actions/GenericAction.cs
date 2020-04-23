using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GenericAction: WorldAction {

    public int executionTime;
    public List<Condition> preconditions;
    public List<Outcome> potentialEffects;

    public GenericAction(string id, int executionTime, List<Condition> preconditions, List<Outcome> potentialEffects):base(id)
    {
        this.executionTime = executionTime;
        this.preconditions = new List<Condition>(preconditions);
        this.potentialEffects = new List<Outcome>(potentialEffects);
        name = ToString();
    }

    public override string ToString()
    {
        return "<"+Id+"(generic)>";
    }

    public bool SatisfiedPreconditions(Person actor, Feature feature, Location location, WorldState ws)
    {
        List<Condition> boundConditions = GetBoundConditions(ws, actor.Id, feature.Id, location.Id);

        return !boundConditions.Exists(condition => !condition.InEffect(actor, feature, location, ws));
    }


    public List<Condition> GetBoundConditions(WorldState ws, string ActorId, string FeatureId, string LocationId)
    {
        Dictionary<string, List<string>> resources = GetActionResources(ws.map, ws.registry.GetPerson(ActorId), FeatureId, LocationId);

        List<Condition> boundConditions = new List<Condition>();

        foreach(Condition condition in preconditions) {
            if (condition is Condition_IsState) {
                Condition_IsState stateCondition = (Condition_IsState)condition;
                boundConditions.Add(new Condition_IsState(stateCondition.state.BindEffect(resources)));
            } else {
                boundConditions.Add(condition);
            }
        }

        return boundConditions;
    }

    public List<Outcome> GenerateExpectedEffects(WorldState ws, string ActorId, string FeatureId, string LocationId)
    {
        Dictionary<string, List<string>> resources = GetActionResources(ws.map, ws.registry.GetPerson(ActorId), FeatureId, LocationId);

        return GenerateKnownEffects(resources);
    }

    List<Outcome> GenerateKnownEffects(Dictionary<string, List<string>> resources)
    {
        return new List<Outcome>(
            from effect in potentialEffects
            select new Outcome(effect.chanceModifier, effect.BindEffects(resources))
        );
    }

    Dictionary<string, List<string>> AggregateResources(Dictionary<string, List<string>> locationResources,
                                                        Dictionary<string, List<string>> featureResources)
    {
        Dictionary<string, List<string>> resources = new Dictionary<string, List<string>>();
        foreach (KeyValuePair<string, List<string>> kp in featureResources) {
            string key = kp.Key;
            List<string> value = Effect.BindId(kp.Value, locationResources);
            resources.Add(key, value);
        }
        return resources;
    }

    Dictionary<string, List<string>> CopyParticipantData(Dictionary<string, List<string>> mainResources,
                                                            Dictionary<string, List<string>> participantData)
    {

        mainResources = AggregateResources(participantData, mainResources);

        foreach (var data in participantData) {
            if (mainResources.ContainsKey(data.Key))
                Debug.Log("key: " + data.Key +
                    " {main: " + string.Join(",", mainResources[data.Key]) +
                    "} {data: " + string.Join(",", data.Value) + "}");

            mainResources.Add(data.Key, data.Value);
        }

        return mainResources;
    }

    Dictionary<string, List<string>> GetActionResources(Map map, Person actor, string FeatureId, string LocationId)
    {
        Feature feature = map.GetFeature(FeatureId);
        Location location = map.GetLocation(LocationId);

        Dictionary<string, List<string>> resources = AggregateResources(location.resources, feature.relevantResources);
        resources = CopyParticipantData(resources, actor.resources);

        return resources;
    }
}

