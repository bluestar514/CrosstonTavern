using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Feature 
{
    [SerializeField]
    public string id;
    public string location;

    public List<GenericAction> providedActions;
    public FeatureResources relevantResources;

    public Inventory inventory;
    public StringIntDictionary stockTable;
    public Skill skill;

    public StatusEffectTable statusEffectTable;

    public int maxUsers;
    public int currentUsers= 0;

    public readonly FeatureType type;
    public enum FeatureType
    {
        river,
        door,
        shop,
        kitchen,
        brush,
        person,
        meadow,
        animal,
        field,
        SYSTEM
    }

    public Feature(string id, FeatureType type, string location, int maxUsers, List<GenericAction> providedActions, 
        Dictionary<string, List<string>> relevantResources, Dictionary<string, int> stockTable=null)
    {
        this.id = id;
        this.type = type;
        this.location = location;
        this.providedActions = providedActions;
        StringStringListDictionary resources = new StringStringListDictionary();
        resources.CopyFrom(relevantResources);
        this.relevantResources = new FeatureResources(resources);

        this.maxUsers = maxUsers;
        this.stockTable = new StringIntDictionary();
        if(stockTable != null) this.stockTable.CopyFrom(stockTable);

        inventory = new Inventory(id);
        statusEffectTable = new StatusEffectTable();

        skill = new Skill();
    }

    public virtual Feature Copy(bool perfect = true)
    {
        Feature f;
        if (perfect) {
            f = new Feature(id, type, location, maxUsers, providedActions, new Dictionary<string, List<string>>(relevantResources.resources), new Dictionary<string, int>(stockTable));
        } else {
            f = new Feature(id, type, location, maxUsers, providedActions, new Dictionary<string, List<string>>(), new Dictionary<string, int>(stockTable));
        }

        f.inventory = inventory.Copy(perfect);
        f.statusEffectTable = statusEffectTable.Copy();
        f.skill = skill.Copy(perfect);

        return f;
    }

    public void Use()
    {
        currentUsers++;
    }

    public void FinishUse()
    {
        currentUsers--;
    }
}