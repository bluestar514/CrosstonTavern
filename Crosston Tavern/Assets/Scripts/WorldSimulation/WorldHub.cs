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

    private void Start()
    {
        InitalizeWorld();

        
        foreach(GenericAction action in GatherProvidedActionsFor("Alicia")) {
            print(action);
        }
    }

    void InitalizeWorld()
    {
        wi = new WorldInitializer();
        locations = wi.InitializeLocations();
        actions = wi.InitializeActions();
        features = wi.InitializeFeatures(actions);
        people = wi.InitializePeople(features, actions);
    }

    Person GetPerson(string id)
    {
        foreach (Person person in people) {
            if (person.Id == id) return person;
        }

        return null;
    }
    bool Neighboring(string source, string dest)
    {
        if (!locations.ContainsKey(source)) throw new Exception("Location " + source + " does not exist");


        return locations[source].resources["connectedSpaces"].Contains(dest);
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
