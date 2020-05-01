using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_HasRelations : State
{
    public int numberRelations;
    public Relationship.CodifiedRelationships relationship;

    public State_HasRelations(int numberRelations, Relationship.CodifiedRelationships relationship)
    {
        this.numberRelations = numberRelations;
        this.relationship = relationship;
    }

    public override List<Effect> MakeActionable(WorldState ws, Person actor)
    {
        IEnumerable<string> people =  from p in ws.registry.GetPeople()
                              where !actor.relationships.HasRelation(p.Id, relationship)
                              select p.Id;

        List<Effect> actionableStates = new List<Effect>();

        foreach (Relationship.RelationType axis in Enum.GetValues(typeof(Relationship.RelationType))) {
            float[] values = Relationship.codifiedRelationRanges[relationship][axis];
            float min = values[0];
            float max = values[1];


            actionableStates.AddRange(from p in people
                                      select new EffectSocialChange((int)min, (int)max,
                                               actor.Id, p, axis));
        }

        return actionableStates;
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        IEnumerable<string> people = from person in actor.relationships.GetKnownPeople()
                                     where actor.relationships.HasRelation(person, relationship)
                                     select person;

        return people.Count() > numberRelations;
    }

    public override string ToString()
    {
        return "<HaveRelationships("+numberRelations+", "+relationship+")>:";
    }
}
