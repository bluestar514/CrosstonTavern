using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldHub : MonoBehaviour
{
    Dictionary<string, Location> locations;
    public List<Person> people;
    Dictionary<string, GenericAction> actions;
    public List<Feature> features;

    WorldInitializer wi;

    public List<WeightedAction> weightedActions = new List<WeightedAction>();

    private void Start()
    {
        InitalizeWorld();


        //foreach(GenericAction action in GatherProvidedActionsFor("Alicia")) {
        //    print(action);
        //}


        //Dictionary<string, List<string>> featureResourses = features[0].relevantResources;
        //Dictionary<string, List<string>> locationResources = locations["farm"].resources;
        //featureResourses = AggregateResources(locationResources, featureResourses);
        //foreach (KeyValuePair<string, List<string>> kp in featureResourses) {
        //    print(kp.Key + ":" + String.Join(", ", kp.Value));
        //}

        foreach(Person person in people) {
            List<BoundAction> boundActions = GatherProvidedActionsFor(person.Id);
            boundActions = FilterOnPrecondition(boundActions);
            List<WeightedAction> weightedActions = WeighOptions(boundActions);

            this.weightedActions.AddRange(weightedActions);
        }
    }

    void InitalizeWorld()
    {
        wi = new WorldInitializer();
        locations = wi.InitializeLocations();
        actions = wi.InitializeActions();
        features = wi.InitializeFeatures(actions, locations);
        people = wi.InitializePeople(features, actions);
    }

    Person GetPerson(string id)
    {
        foreach (Person person in people) {
            if (person.Id == id) return person;
        }

        return null;
    }
    Feature GetFeature(string id)
    {
        foreach (Feature feature in features) {
            if (feature.Id == id) return feature;
        }

        return null;
    }

    bool Neighboring(string source, string dest)
    {
        if (!locations.ContainsKey(source)) throw new Exception("Location " + source + " does not exist");

        return locations[source].resources["connectedLocation"].Contains(dest);
    }

    List<Feature> GatherFeaturesAt(string locationId)
    {
        List<Feature> nearbyFeatures = new List<Feature>(from feature in features
                                                         where feature.location == locationId
                                                         select feature);
        return nearbyFeatures;
    }

    List<BoundAction> GatherProvidedActionsFor(string actorId)
    {
        Person person = GetPerson(actorId);
        if (person == null) throw new Exception("Person " + actorId + " does not exist");

        List<Feature> nearByFeatures = GatherFeaturesAt(person.location);

        List<BoundAction> availableActions = new List<BoundAction>();
        foreach (Feature feature in nearByFeatures) {

            availableActions.AddRange(from action in feature.providedActions
                                      select new BoundAction(action, actorId, feature.Id, person.location));
        }

        return availableActions;
    }

    List<BoundAction> FilterOnPrecondition(List<BoundAction> actions)
    {

        return new List<BoundAction>(from action in actions
                                     where action.SatisfiedPreconditions(
                                         GetPerson(action.ActorId),
                                         GetFeature(action.FeatureId),
                                         locations[action.LocationId])
                                     select action);

    }

    Dictionary<string, List<string>> AggregateResources(Dictionary<string, List<string>> locationResources, 
        Dictionary<string, List<string>> featureResources)
    {
        Dictionary<string, List<string>> resources = new Dictionary<string, List<string>>();
        foreach(KeyValuePair<string, List<string>> kp in featureResources) {
            string key = kp.Key;
            List<string> value =  MicroEffect.BindId(kp.Value, locationResources);
            resources.Add(key, value);
        }
        return resources;
    }

    List<WeightedAction> WeighOptions(List<BoundAction> actions)
    {
        List<WeightedAction> weightedActions = new List<WeightedAction>();

        foreach(BoundAction action in actions) {
            Person actor = GetPerson(action.ActorId);
            Feature feature = GetFeature(action.FeatureId);
            Location location = locations[action.LocationId];

            Dictionary<string, List<string>> resources = 
                AggregateResources(location.resources, feature.relevantResources);

            List<Effect> boundEffects = action.GenerateKnownEffects(resources);

            weightedActions.Add(actor.EvaluateAction(action, boundEffects));
        }

        return weightedActions;
    }

    void MovePerson(string personId, string locationId, bool respectDoors=true)
    {
        Person person = GetPerson(personId);
        if (person == null) throw new Exception("Person " + personId + " does not exist");
        if(!locations.ContainsKey(locationId)) throw new Exception("Location " + locationId + " does not exist");

        if (!respectDoors || Neighboring(person.location, locationId)) {
            person.location = locationId;
            person.feature.location = locationId;
        }

    }



}
