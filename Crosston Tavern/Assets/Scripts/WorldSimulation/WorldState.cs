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
    private WorldTime time;
    public KnownFacts knownFacts;

    public WorldTime Time { get => new WorldTime(time); set => time = value; }

    public WorldState(Map map, Registry registry, WorldTime time, string owner)
    {
        this.map = map;
        this.registry = registry;
        this.Time = time;
        knownFacts = new KnownFacts(owner);
    }

    public WorldState Copy(Person owner, string name)
    {
        Map c_map = map.Copy(name);
        c_map.RemoveFeature(owner.id);
        c_map.AddFeature(owner.id, owner.Copy(true));

        Registry c_reg = new Registry(c_map.GetAllFeatures(), name);

        WorldState copy = new WorldState(c_map, c_reg, Time, name);
        copy.id = name;
        return copy;
    }


    public void Tick(int t=1)
    {
        Time.Tick(t);
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


    public void AddHistory(ExecutedAction action)
    {
        knownFacts.AddHistory(action, this);
    }
}
