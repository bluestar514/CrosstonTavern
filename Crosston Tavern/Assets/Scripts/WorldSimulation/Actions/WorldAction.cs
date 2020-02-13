using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldAction
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }



    public WorldAction(string id)
    {
        Id = id;
    }
}
