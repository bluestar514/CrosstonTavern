﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerbilizationAction : VerbilizationInfo
{
    protected string verbPresent;
    protected string verbPast;

    public VerbilizationAction(string verbPresent, string verbPast)
    {
        this.verbPresent = verbPresent;
        this.verbPast = verbPast;
    }

    public virtual string Verbilize(string actor, string feature, bool presentTense)
    {
        return verbPresent;
    }
}

public class VerbilizationMovement : VerbilizationAction
{
    public VerbilizationMovement() : base(" go to ", " went to ")
    {
    }

    // bob, door_farm->feild, true
    public override string Verbilize(string actor, string feature, bool presentTense)
    {
        string went = verbPast;
        if (presentTense) went = verbPresent;
        return actor + went + feature.Split('>')[1];
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
    public override string Verbilize(string actor, string feature, bool presentTense)
    {
        string verb = verbPresent;
        if (!presentTense) verb = verbPast;

        return actor + " " + verb + " at " + feature;
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
    public override string Verbilize(string actor, string feature, bool presentTense)
    {
        string verb = verbPresent;
        if (!presentTense) verb = verbPast;

        return actor + " " + verb + " " + feature;
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
    public override string Verbilize(string actor, string feature, bool presentTense)
    {
        return base.Verbilize(actor, feature, presentTense);
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
    public override string Verbilize(string actor, string feature, bool presentTense)
    {
        string verb = verbPresent;
        if (!presentTense) verb = verbPast;

        return actor + " " + verb + " " + feature + " for a " + itemBinding;
    }
}

public class VerbilizationActionItemGive : VerbilizationActionItem
{
    public VerbilizationActionItemGive(string verbPresent, string verbPast, string itemBinding) : base(verbPresent, verbPast, itemBinding)
    {
    }

    //I gave Alicia a strawberry
    //Bob gave Alicia a cake
    public override string Verbilize(string actor, string feature, bool presentTense)
    {
        string verb = verbPresent;
        if (!presentTense) verb = verbPast;

        return actor + " " + verb + " " + feature + " a " + itemBinding;
    }
}