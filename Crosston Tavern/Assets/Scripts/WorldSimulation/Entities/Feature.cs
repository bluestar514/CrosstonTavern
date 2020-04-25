using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Feature 
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }
    public string location;

    public List<GenericAction> providedActions;
    public StringStringListDictionary relevantResources;
    public Inventory inventory;
    public StringIntDictionary stockTable;

    public int maxUsers;
    public int currentUsers= 0;

    public Feature(string id, string location, int maxUsers, List<GenericAction> providedActions, 
        Dictionary<string, List<string>> relevantResources, Dictionary<string, int> stockTable=null)
    {
        Id = id;
        this.location = location;
        this.providedActions = providedActions;
        this.relevantResources = new StringStringListDictionary();
        this.relevantResources.CopyFrom(relevantResources);
        this.relevantResources.Add("recipient", new List<string>() { id });
        this.maxUsers = maxUsers;
        this.stockTable = new StringIntDictionary();
        if(stockTable != null) this.stockTable.CopyFrom(stockTable);

        inventory = new Inventory(id);
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