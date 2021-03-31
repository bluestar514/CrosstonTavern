using System.Collections.Generic;

public class StateProfession: State
{
    public string townieId;
    public string professionId;

    public StateProfession(string townieId, string professionId)
    {
        this.townieId = townieId;
        this.professionId = professionId;

        id = ToString();
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string townieId = bindings.BindString(this.townieId);
        string professionId = bindings.BindString(this.professionId);

        return new StateProfession(townieId, professionId);
    }

    public override List<State> Combine(State state)
    {
        if (this.Equals(state))
            return new List<State>() { this };
        else
            return new List<State>() { this, state };
    }

    public override bool Equals(object obj)
    {
        return
            obj is State state &&
            state is StateProfession stateProfession &&
            stateProfession.professionId == professionId &&
            stateProfession.townieId == townieId;
    }

    public override int GetHashCode()
    {
        int hashCode = 1367305302;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(townieId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(professionId);
        return hashCode;
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        StateProfession state = (StateProfession)Bind(bindings, resources);
        
        Person person = ws.map.GetPerson(state.townieId);
        if (person == null) throw new System.Exception("Person (" + state.townieId + ") not found in WorldState(" + ws.id + ")");

        return person.profession == state.professionId;
    }

    public override string ToString()
    {
        return "<StateProfession(" + townieId + "-" + professionId + ")>";
    }

    public override string Verbalize(string speakerId, string listenerId, bool goal, bool futureTense = true)
    {
        return townieId + " is a " + professionId;
    }
}