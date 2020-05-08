using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldState 
{
    public Map map;
    public Registry registry;
    public WorldTime time;

    public WorldState(Map map, Registry registry, WorldTime time)
    {
        this.map = map;
        this.registry = registry;
        this.time = time;
    }


    public void Tick(int t=1)
    {
        time.Tick(t);
    }


    public Inventory GetInventory(string id)
    {
        if(registry.GetPerson(id) != null) {
            return registry.GetPerson(id).inventory;
        }
        if(map.GetFeature(id) != null) {
            return map.GetFeature(id).inventory;
        }

        Debug.LogWarning("Could not find an inventory for " + id);

        return null;
    }

    public Relationship GetRelationshipsFor(string id)
    {
        if (id.StartsWith("person_")) id = id.Replace("person_", "");

        if (registry.GetPerson(id) != null) {
            return registry.GetPerson(id).relationships;
        }

        Debug.LogWarning("Could not find " + id);

        return null;
    }

    public List<string> GetBystanders(string locationId)
    {
        return new List<string>(from person in map.GatherFeaturesAt(locationId)
                                where person.Id.StartsWith("person_")
                                select person.Id.Replace("person_", ""));
    }
}
