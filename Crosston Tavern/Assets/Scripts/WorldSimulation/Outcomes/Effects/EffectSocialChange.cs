using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSocialChange : Effect
{
    public int DeltaMin { get; private set; }
    public int DeltaMax { get; private set; }
    public string SourceId { get; private set; }
    public string TargetId { get; private set; }
    public Relationship.RelationType RelationType { get; private set; }

    public EffectSocialChange(int deltaMin, int deltaMax, string sourceId, string targetId, Relationship.RelationType relationType)
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;
        SourceId = sourceId;
        TargetId = targetId;
        RelationType = relationType;

        id = ToString();
    }

    public override Effect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new EffectSocialChange(DeltaMin, DeltaMax,
                                BindId(SourceId, resources),
                                BindId(TargetId, resources),
                                RelationType);
    }

    public override Effect SpecifyEffect()
    {
        int randNum = UnityEngine.Random.Range(DeltaMin, DeltaMax + 1);

        return new EffectSocialChange(randNum, randNum, SourceId, TargetId, RelationType);
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        float val = ws.GetRelationshipsFor(SourceId).Get(TargetId, RelationType);

        return val >= DeltaMin && val <= DeltaMax;
    }

    public override string ToString()
    {
        return "<SocialChange({" + DeltaMin + "~" + DeltaMax + "}, " + SourceId + "-(" + RelationType.ToString() + ")->" + TargetId + ")>";
    }
}