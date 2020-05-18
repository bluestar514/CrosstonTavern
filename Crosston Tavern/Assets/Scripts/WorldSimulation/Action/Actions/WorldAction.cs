using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldAction
{

    [SerializeField]
    protected string name;

    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }

    public int executionTime;
    public Precondition preconditions;
    public List<Outcome> potentialOutcomes;

    public WorldAction(string id, int executionTime, Precondition preconditions, List<Outcome> potentialEffects)
    {
        Id = id;
        name = Id;

        this.executionTime = executionTime;
        this.preconditions = preconditions;
        this.potentialOutcomes = new List<Outcome>(potentialEffects);
    }
}
