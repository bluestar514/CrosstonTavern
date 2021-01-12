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
    [SerializeField] private WorldTime time;
    public KnownFacts knownFacts;

    public WorldTime Time { get => new WorldTime(time); set => time = value; }
    public List<string> completeItemsList;

    public WorldState(Map map, Registry registry, WorldTime time, string owner)
    {
        this.map = map;
        this.registry = registry;
        this.Time = time;
        knownFacts = new KnownFacts(owner);

        completeItemsList = GenerateFullItemList();
    }

    public WorldState Copy(Person owner, string name)
    {
        Map c_map = map.Copy(name);
        c_map.RemoveFeature(owner.id);
        c_map.AddFeature(owner.id, owner.Copy(true));

        Registry c_reg = new Registry(c_map.GetAllFeatures(), name);

        WorldState copy = new WorldState(c_map, c_reg, time, name);
        copy.id = name;
        return copy;
    }


    public void Tick(int t=1)
    {
        time.Tick(t);
    }

    public void NewDay()
    {
        time.AdvanceToStartOfDay();
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

    public Skill GetSkillFor(string id)
    {
        if (id.StartsWith("person_")) id = id.Replace("person_", "");

        if (registry.GetPerson(id) != null) {
            return registry.GetPerson(id).skill;
        }

        Debug.LogWarning("Could not find " + id);

        return null;
    }

    public List<string> GetBystanders(string locationId)
    {
        return new List<string>(from person in map.GatherFeaturesAt(locationId)
                                where person.type == Feature.FeatureType.person
                                select person.id);
    }


    public void AddHistory(ExecutedAction action)
    {
        knownFacts.AddHistory(action, this);
    }

    public void LearnFact(WorldFact fact)
    {
        knownFacts.AddFact(fact, this);
    }


    private List<string> GenerateFullItemList()
    {
        HashSet<string> allItems = new HashSet<string>(ActionInitializer.GetAllActionGeneratedItems());

        foreach(Feature f in map.GetAllFeatures()) {
            foreach(KeyValuePair<string, List<string>> p in f.relevantResources.resources) {
                if (p.Key == ResourceCatagories.r_connectedLocation) continue;

                List<string> itemList = p.Value;
                foreach(string item in itemList) {
                    allItems.Add(item);
                }
            }
        }


        foreach(Person person in registry.GetPeople()) {
            foreach(string item in person.inventory.GetItemList()) {
                allItems.Add(item);
            }
        }

        return new List<string>(allItems);
    }
}
