using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerbilizationInfo
{
    //fishing at the lake in the forest
    //complimented alicia at the inn

    public string verb; //fishing, complimented, etc

    public VerbilizationInfo(string verb)
    {
        this.verb = verb;
    }

    public virtual string Verbalize(ExecutedAction action)
    {
        return action.ToString();
    }
}

public class VerbalizePatternFishing : VerbilizationInfo
{
    //Why did bob go fishing at the lake in the forest?
    //I went foraging in the brush in the feild.

    public string inAt;

    public VerbalizePatternFishing(string verb, string inAt) : base(verb)
    {
        this.inAt = inAt;
    }

    public override string Verbalize(ExecutedAction action)
    {
        return verb + " " + inAt + " the " + action.Action.FeatureId + " in the " + action.Action.LocationId;
    }
}

public class VerbalizePatternCompliment : VerbilizationInfo
{
    public VerbalizePatternCompliment(string verb) : base(verb)
    {
    }

    //Why did bob compliment alicia in the inn?
    //I complimented alicia in the farm. 

    public override string Verbalize(ExecutedAction action)
    {
        return verb + action.Action.FeatureId + " in the " + action.Action.LocationId;
    }
}

public class VerbalizePatternMove : VerbilizationInfo
{
    public VerbalizePatternMove() : base("went")
    {
    }

    public override string Verbalize(ExecutedAction action)
    { //why did you go
        //I went 
        return " to the " + action.Action.LocationId;
    }
}

public class VerbalizationPatternItem: VerbilizationInfo
{
    //I gave alicia a cake
    //Why did you give alicia a cake
    //I did bob ask alicia for a strawberry?
    string optionalFor;

    public VerbalizationPatternItem(string verb, string optionalFor) : base(verb)
    {
        this.optionalFor = optionalFor;
    }

    public override string Verbalize(ExecutedAction action)
    {
        string item = action.Action.Bindings.BindString("#item#");

        return verb + " " + action.Action.FeatureId+ " "+ optionalFor + " a " + item;
    }
}