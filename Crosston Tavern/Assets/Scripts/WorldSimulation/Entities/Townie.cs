using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Townie: MonoBehaviour
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }

    public Person townieInformation; //should be them as a person in the WS;

    public WorldState ws; //there understanding of the WorldState

    public GoalManager gm;
    
    public string homeLocation;

    public void TownieInit(Person townieInformation, WorldState ws, GoalManager gm)
    {
        id = townieInformation.id;
        this.townieInformation = townieInformation;
        this.ws = ws;
        this.gm = gm;
    }


    /// <summary>
    /// Updates the location of a person as far as a particular Townie is aware of their location
    /// </summary>
    /// <param name="moverId"> The one actually moving</param>
    /// <param name="newLocationId"> Where they are ending up</param>
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
