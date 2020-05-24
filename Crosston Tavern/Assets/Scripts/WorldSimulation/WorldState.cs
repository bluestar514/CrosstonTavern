using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldState 
{
    public string id = "default";
    public Map map;
    public Registry registry;
    public WorldTime time;

    public WorldState(Map map, Registry registry, WorldTime time)
    {
        this.map = map;
        this.registry = registry;
        this.time = time;
    }

    public WorldState Copy(string name = "copy")
    {
        Map c_map = map.Copy(name);
        Registry c_reg = new Registry(c_map.GetAllFeatures(), name);

        WorldState copy = new WorldState(c_map, c_reg, time);
        copy.id = name;
        return copy;
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
                                where person.id.StartsWith("person_")
                                select person.id.Replace("person_", ""));
    }
}
