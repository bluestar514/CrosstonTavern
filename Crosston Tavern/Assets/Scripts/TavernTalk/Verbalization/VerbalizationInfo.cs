using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerbilizationInfo
{

}

public class VerblilizationFeature : VerbilizationInfo
{
    string feature;

    public VerblilizationFeature(string feature)
    {
        this.feature = feature;
    }

    public string Verbilize()
    {
        return feature;
    }
}