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

    public List<ExecutedAction> history;

    public Townie(Person townieInformation, WorldState ws, GoalManager gm)
    {
        id = townieInformation.id;
        this.townieInformation = townieInformation;
        this.ws = ws;
        this.gm = gm;
        history = new List<ExecutedAction>();
    }



    public void Move(string moverId, string newLocationId)
    {
        if(moverId == id) {
            townieInformation.location = newLocationId;
        }

        ws.map.MovePerson(moverId, newLocationId, false);

    }

    public override string ToString()
    {
        return townieInformation.ToString();
    }
}
