using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerbilizationAction : VerbilizationInfo
{
    public string verbPresent;
    public string verbPast;

    protected string verb;
    protected string actor;
    protected string feature;


    public VerbilizationAction(string verbPresent, string verbPast)
    {
        this.verbPresent = verbPresent;
        this.verbPast = verbPast;
    }

    public virtual string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        verb = verbPresent;
        if (!presentTense) verb = verbPast;

        if (includeSubject)
            this.actor = VerbalizationDictionary.Replace(actor);

        this.feature = VerbalizationDictionary.Replace(feature);

        return verbPresent;
    }
}

public class VerbilizationMovement : VerbilizationAction
{
    public VerbilizationMovement() : base(" go to ", " went to ")
    {
    }

    // bob, door_farm->feild, true
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);

        this.feature = VerbalizationDictionary.Replace(feature.Split('>')[1]);
        if (includeSubject)
            return this.actor + this.verb + this.feature;
        else
            return this.verb + this.feature;
    }
}

public class VerbilizationActionResourceGathering : VerbilizationAction
{
    public VerbilizationActionResourceGathering(string verbPresent, string verbPast) : base(verbPresent, verbPast)
    {
    }


    //I went fishing at the pond in the forest
    //you went foraging at the feild bushes
    //Bob went fishing at the farm river
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);
        if (includeSubject)
            return this.actor + " " + this.verb + " at " + this.feature;
        else
            return this.verb + " at " + this.feature;
    }
}

public class VerbilizationActionSocial : VerbilizationAction
{
    public VerbilizationActionSocial(string verbPresent, string verbPast) : base(verbPresent, verbPast)
    {
    }

    //I complimented Alicia
    //you insulted Alicia
    //Bob started dating Alicia
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);
        if (includeSubject)
            return this.actor + " " + this.verb + " " + this.feature;
        else
            return this.verb + " " + this.feature; 
    }
}

/// <summary>
/// Assumes singular items presently
/// </summary>
public class VerbilizationActionItem : VerbilizationAction
{
    protected string itemBinding;

    public VerbilizationActionItem(string verbPresent, string verbPast, string itemBinding) : base(verbPresent, verbPast)
    {
        this.itemBinding = itemBinding;
    }

    //I gave Alicia a strawberry
    //you asked Alicia for a rose
    //Bob gave Alicia a cake
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        itemBinding = VerbalizationDictionary.Replace(itemBinding, VerbalizationDictionary.Form.singular);

        return base.Verbilize(actor, feature, presentTense, includeSubject);
    }
}

public class VerbilizationActionItemAskFor : VerbilizationActionItem
{
    public VerbilizationActionItemAskFor(string verbPresent, string verbPast, string itemBinding) : base(verbPresent, verbPast, itemBinding)
    {
    }

    //I gave Alicia a strawberry
    //you asked Alicia for a rose
    //Bob gave Alicia a cake
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);
        string item = VerbalizationDictionary.Replace(itemBinding);

        if(includeSubject)
            return this.actor + " " + this.verb + " " + this.feature + " for a " + item;
        else
            return this.verb + " " + this.feature + " for a " + item;
    }
}

public class VerbilizationActionItemGive : VerbilizationActionItem
{
    public VerbilizationActionItemGive(string verbPresent, string verbPast, string itemBinding) : base(verbPresent, verbPast, itemBinding)
    {
    }

    //I gave Alicia a strawberry
    //Bob gave Alicia a cake
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);
        string item = VerbalizationDictionary.Replace(itemBinding);

        return this.actor + " " + this.verb + " " + this.feature + " a " + item;
    }
}

public class VerbalizationActionFeatureAt : VerbilizationAction
{
    public VerbalizationActionFeatureAt(string verbPresent, string verbPast) : base(verbPresent, verbPast){}

    public override string ToString()
    {
        return base.ToString();
    }

    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);
        if(includeSubject)  
            return this.actor + " " + this.verb+ " at "+ this.feature;
        else
            return this.verb + " at " + this.feature; ;
    }
}

public class VerbalizationActionCooking : VerbilizationActionItem
{

    public VerbalizationActionCooking(string verbPresent, string verbPast, string item) : base(verbPresent, verbPast,  item) { }

    public override string ToString()
    {
        return base.ToString();
    }

    //I made trout stew in Bob's Kitchen
    public override string Verbilize(string actor, string feature, bool presentTense, bool includeSubject = true)
    {
        base.Verbilize(actor, feature, presentTense, includeSubject);
        if (includeSubject)
            return this.actor + " " + this.verb + " "+ itemBinding + " at " + this.feature;
        else
            return this.verb + " " + itemBinding + " at " + this.feature; ;
    }
}