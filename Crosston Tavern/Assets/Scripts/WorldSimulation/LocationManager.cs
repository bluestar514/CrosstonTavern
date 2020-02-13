using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class LocationManager 
{
    List<Feature> features;
    Dictionary<string, Location> locations;

    public LocationManager(List<Feature> features, Dictionary<string, Location> locations)
    {
        this.features = features;
        this.locations = locations;
    }


}
