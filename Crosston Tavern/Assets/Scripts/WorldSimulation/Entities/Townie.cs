using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Townie
{
    [SerializeField]
    private string id;


    public Person townieInformation; //should be them as a person in the WS;

    public WorldState ws; //there understanding of the WorldState

    public GoalManager gm;

    public Townie(Person townieInformation, WorldState ws, GoalManager gm)
    {
        id = townieInformation.id;
        this.townieInformation = townieInformation;
        this.ws = ws;
        this.gm = gm;
    }
}
