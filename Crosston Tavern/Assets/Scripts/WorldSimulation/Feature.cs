﻿using System.Collections;
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

    public Feature(string id, string location, List<GenericAction> providedActions, Dictionary<string, List<string>> relevantResources)
    {
        Id = id;
        this.location = location;
        this.providedActions = providedActions;
        this.relevantResources = new StringStringListDictionary();
        this.relevantResources.CopyFrom(relevantResources);
    }
}
