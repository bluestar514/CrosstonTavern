using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Townie: MonoBehaviour
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }

    public Person townieInformation { get => ws.map.GetPerson(id);}

    public WorldState ws; //their understanding of the WorldState

    public GoalManager gm;
    
    public string homeLocation;

    public void TownieInit(string id, WorldState ws)
    {
        this.id = id;
        this.ws = ws;
        this.gm = new GoalManager(ws, townieInformation);
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


    public bool KnowsAboutAction(BoundAction potentialAction)
    {
        ActionBuilder ab = new ActionBuilder(ws, townieInformation);

        List<BoundAction> allActions = ab.GetAllActions(respectLocation: false);

        return allActions.Contains(potentialAction);
    }
}
