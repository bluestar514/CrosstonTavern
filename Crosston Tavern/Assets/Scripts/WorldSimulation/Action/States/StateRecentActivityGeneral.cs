using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StateRecentActivity : State
{
    public string genericActionId; //what action are we looking for
    public WorldTime interval; // in what time interval
    public int countMin;
    public int countMax;
    public bool want; // if true, this activity has happened in the interval the specified number of times
                      // if false, this activity has not happened in the interval the specified number of times


    public string actorId;
    public string featureId;
    public string location;
    public BoundBindingCollection otherBindings;


    public StateRecentActivity(string genericActionId, WorldTime interval, int countMin, int countMax, bool want,
        string actorId = null, string featureId = null, string location = null, BoundBindingCollection otherBindings = null)
    {
        this.genericActionId = genericActionId;
        this.interval = interval;
        this.countMin = countMin;
        this.countMax = countMax;
        this.want = want;

        this.actorId = actorId;
        this.featureId = featureId;
        this.location = location;
        this.otherBindings = otherBindings;

        id = ToString();
    }

    public virtual IEnumerable<ExecutedAction> GetRelevantHistory(WorldState ws, IEnumerable<ExecutedAction> initialHistory, BoundBindingCollection bindings = null)
    {
        IEnumerable<ExecutedAction> history = initialHistory;

        string genericAction = this.genericActionId;
        string actorId = this.actorId;
        string featureId = this.featureId;
        string location = this.location;
        if (bindings != null) {
            genericAction = bindings.BindString(genericActionId);
            actorId = bindings.BindString(this.actorId);
            featureId = bindings.BindString(this.featureId);
            location = bindings.BindString(this.location);
        }

        history = from action in history
                  where (actorId == null || action.Action.ActorId == actorId) &&
                        (featureId == null || action.Action.FeatureId == featureId) &&
                        (location == null || action.Action.LocationId == location)
                  select action;



        history = from action in history
                  where action.executionTime.InTimeInterval(ws.Time, interval)
                  where genericAction == action.Action.Bindings.BindString(action.Action.Id)
                  select action;

        return history;
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        IEnumerable<ExecutedAction> history = ws.knownFacts.GetHistory();

        history = GetRelevantHistory(ws, history, bindings);

        return (history.Count() <= countMax && history.Count() >= countMin) == want;
    }

    public override List<State> Combine(State state)
    {
        if(this.Equals(state)) {
            return new List<State>() { this };
        }
        return new List<State>() { this, state };
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string genericAction = bindings.BindString(this.genericActionId);

        string actorId = bindings.BindString(this.actorId);
        string featureId = bindings.BindString(this.featureId);
        string location = bindings.BindString(this.location);

        return new StateRecentActivity(genericAction, interval, countMin, countMax, want,
                                                actorId, featureId, location, otherBindings);
    }

    public override bool Equals(object obj)
    {
        if (obj is StateRecentActivity recentState) {
            return this.genericActionId == recentState.genericActionId &&
                this.interval == recentState.interval &&
                this.countMin == recentState.countMin &&
                this.countMax == recentState.countMax &&
                this.want == recentState.want &&
                (actorId == null || recentState.actorId == actorId) &&
                (featureId == null || recentState.featureId == featureId) &&
                (location == null || recentState.location == location);
        }

        return false;
    }



    public override string ToString()
    {
        return "<StateRecentActivity(" + genericActionId + "(" + actorId+", "+featureId+"," +location + ",{" + otherBindings+"}) "+
            "<"+interval + ">:" + countMin +"~"+countMax + "-"+want + ")>";
    }

    public override int GetHashCode()
    {
        int hashCode = 28156220;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(genericActionId);
        hashCode = hashCode * -1521134295 + EqualityComparer<WorldTime>.Default.GetHashCode(interval);
        hashCode = hashCode * -1521134295 + countMin.GetHashCode();
        hashCode = hashCode * -1521134295 + countMax.GetHashCode();
        hashCode = hashCode * -1521134295 + want.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(actorId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(featureId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(location);
        hashCode = hashCode * -1521134295 + EqualityComparer<BoundBindingCollection>.Default.GetHashCode(otherBindings);
        return hashCode;
    }
}


