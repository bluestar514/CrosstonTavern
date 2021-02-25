using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect 
{
    public string id;
    public VerbilizationEffect verbalization;

    public Effect()
    {
        id = ToString();
    }

    public override string ToString()
    {
        return "<Generic Effect>";
    }

    /// <summary>
    /// Used primarily inside the ActionHeuristicManager. Exists inside the Effect Class so each kind of effect can decide how it wants to be handled individually
    /// </summary>
    /// <param name="ws"> World State being used in consideration </param>
    /// <param name="bindings"> Action's bindings</param>
    /// <param name="resources"> Resources on the feature being acted against</param>
    /// <param name="goal"> A goal for the effect to be weighed against</param>
    /// <returns> An estimate of how much a character wants to do something. This value should be between -1 and 1 at all times</returns>
    public virtual float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal) {
        return 0;
    }

    /// <summary>
    /// Used to finish a lot of weight calculations across different kinds of effects. 
    /// </summary>
    /// <param name="count">Total amount of something</param>
    /// <param name="delta">Amount the value changed in this step</param>
    /// <param name="min">Minumum Goal Value</param>
    /// <param name="max">Maximum Goal Value</param>
    /// <returns>Weight based on how much we have moved into the target zone. Value should be between -1 and 1</returns>
    protected float CountInRange(float count, float delta, float min, float max, bool allowNegativeNewCount = true)
    {
        float weight = 0;

        float newCount = count + delta;
        if (!allowNegativeNewCount) newCount = Mathf.Max(0, newCount);

        //get there in one step:
        // in range -> +1
        // outside -> -1
        if (newCount <= max && newCount >= min) weight += 1;
        else weight -= 1; 

        //less important if we are already in range
        // were in range -> -1
        if (count <= max && count >= min) weight -= 1;

        //move in the right direction:
        // if move in right direction from outside range -> +2
        // if move in wrong direction from outside range -> -2
        // if move from inside the range -> 0
        if (Mathf.Abs(newCount - min) <= Mathf.Abs(count - min)) weight += 1;
        else weight -= 1;
        if (Mathf.Abs(newCount - max) <= Mathf.Abs(count - max)) weight += 1;
        else weight -= 1;

        //roughly 5 options: 
        //1. start outside the range, and move into it -> weight = 3
        //2. start outside the range, and move toward it, but not into it -> weight = 2
        //3. start in the range and don't move out of it -> weight = 0
        //4. start outside the range, and move away from it -> weight = -3
        //5. start in the range and move out of it -> weight = -4

        return weight/4;
    }

    /// <summary>
    /// Used Primarily inside the ActionExecutionManager. Exists inside the Effect Class so each kind of effect can decide how it wants to be handled individually
    /// </summary>
    /// <param name="ws"> World State to be altered and acted against</param>
    /// <param name="townie"> Person who made the action</param>
    /// <param name="bindings"> Action's bindings</param>
    /// <param name="resources"> Resources on the feature being acted against</param>
    /// <returns> A Static Effect with all values determined</returns>
    public virtual Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        Debug.LogWarning("Effect (" + this + ") of unaccounted for Effect Type failed to be executed!");

        return new Effect();
    }

    /// <summary>
    /// Combines two Effects into 1 if possible. Returns Null if they cannot be combined.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual Effect Combine(Effect other)
    {
        return null;
    }
}


public class EffectMovement: Effect
{
    public string moverId;
    public string newLocationId;

    public EffectMovement(string moverId, string newLocationId, VerbilizationEffect verbilizationEffect = null)
    {
        this.moverId = moverId;
        this.newLocationId = newLocationId;
        this.verbalization = verbilizationEffect;

        id = ToString();
    }

    public override string ToString()
    {
        return "<EffectMovement(" + moverId + "," + newLocationId + ")>";
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (goal is GoalState goalState && goalState.state is StatePosition) return WeighForLocation(ws, bindings, resources, goalState);
        
        else return WeighToDoAction(ws, bindings, resources, goal);

    }

    private float WeighForLocation(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, GoalState goal)
    {
        StatePosition state = (StatePosition)goal.state;

        string mover = bindings.BindString(moverId);
        string loc = bindings.BindString(newLocationId);

        List<string> locations = resources.BindString(loc);

        Map map = ws.map;

        string currentLocation = ws.map.GetPerson(mover).location;
        string goalLocation = state.locationId;

        if (mover != state.moverId) return 0;

        float weight = 0;

        foreach (string location in locations) {
            
            // if we are there, disinsentivise leaving
            // if we are a step away, incentivise taking that last step
            // generally reward going in the right direction, punish going the wrong way
            // weight varies between 2 and -2
            if (location == goalLocation) weight += 1;
            int currentDist = map.GetDistance(currentLocation, goalLocation);
            int newDist = map.GetDistance(location, goalLocation);
            if (currentDist > newDist) weight += 1;
            if (currentDist < newDist) weight -= 1;
            if (currentLocation == goalLocation) weight -= .5f; //I would like characters to float between equally 
                                                                //good locations a bit more than they were when this was set to 1
        }

        return weight / locations.Count / 2;
        //2 is max score per location, this should normalize the number to always be between 0 and 1;
    }

    private float WeighToDoAction(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        return 0;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {

        string moverId = bindings.BindString(this.moverId);
        string newLocationId = bindings.BindString(this.newLocationId);

        List<string> potentialIds = resources.BindString(newLocationId);
        newLocationId = potentialIds[Mathf.FloorToInt(UnityEngine.Random.value * potentialIds.Count)];

        if (townie != null)  townie.Move(moverId, newLocationId);
        ws.map.MovePerson(moverId, newLocationId, false);

        return new EffectMovement(moverId, newLocationId, verbalization);
    }

}

public class NotAnEffect : Effect
{
    public NotAnEffect(string id, VerbilizationEffect verbilizationEffect = null)
    {
        this.id = id;
        verbalization = verbilizationEffect;
    }

    public override string ToString()
    {
        return "<NotReallyAnEffect:"+id+">";
    }
}