using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager
{

    public static Dictionary<string, List<string>> AggregateResources(Dictionary<string, List<string>> locationResources,
    Dictionary<string, List<string>> featureResources)
    {
        Dictionary<string, List<string>> resources = new Dictionary<string, List<string>>();
        foreach (KeyValuePair<string, List<string>> kp in featureResources) {
            string key = kp.Key;
            List<string> value = MicroEffect.BindId(kp.Value, locationResources);
            resources.Add(key, value);
        }
        return resources;
    }

    public static Dictionary<string, List<string>> CopyParticipantData(Dictionary<string, List<string>> mainResources,
        Dictionary<string, List<string>> participantData)
    {

        mainResources = AggregateResources(participantData, mainResources);

        foreach (var data in participantData) {
            if (mainResources.ContainsKey(data.Key))
                Debug.Log("key: " + data.Key +
                    " {main: " + String.Join(",", mainResources[data.Key]) +
                    "} {data: " + String.Join(",", data.Value) + "}");

            mainResources.Add(data.Key, data.Value);
        }

        return mainResources;
    }

    public static Dictionary<string, List<string>> GetActionResources(Map map, BoundAction action, Person actor)
    {
        Feature feature = map.GetFeature(action.FeatureId);
        Location location = map.GetLocation(action.LocationId);

        Dictionary<string, List<string>> resources = AggregateResources(location.resources, feature.relevantResources);
        resources = CopyParticipantData(resources, actor.resources);

        return resources;
    }

}