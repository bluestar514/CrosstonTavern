using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectVariableSocial : EffectInvChange
{


    public EffectVariableSocial(int deltaMin, int deltaMax, string target, List<string> itemId) : base(deltaMin, deltaMax, target, itemId)
    {
    }

    public EffectSocialChange BuildStaticSocialChange(WorldState ws, Person actor, Person target)
    {
        if (target.preferences["liked"].Contains(ItemId[0])) {
            return new EffectSocialChange(5, 5, target.Id, actor.Id, Relationship.RelationType.friendly);
        }else if(target.preferences["disliked"].Contains(ItemId[0])) {
            return new EffectSocialChange(-5, -5, target.Id, actor.Id, Relationship.RelationType.friendly);
        }

        return new EffectSocialChange(1, 1, target.Id, actor.Id, Relationship.RelationType.friendly);
    } 
}
