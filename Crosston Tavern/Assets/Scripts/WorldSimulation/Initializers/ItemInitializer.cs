using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInitializer 
{
    public static Dictionary<string, FoodItem> menu = new Dictionary<string, FoodItem>() {
        { "strawberry_cake", new FoodItem("strawberry_cake",
                                            new List<string>(){ "strawberry", "egg", "milk" },
                                            new VerbilizationAction("bake", "baked")) },
        { "fried_salmon", new FoodItem("fried_salmon",
                                            new List<string>(){ "salmon", "butter" },
                                            new VerbilizationAction("make", "made")) },
        { "blackberry_tart", new FoodItem("blackberry_tart",
                                            new List<string>(){ "blackberry", "flour", "egg", "butter" },
                                            new VerbilizationAction("bake", "baked")) },
        { "stewed_trout", new FoodItem("stewed_trout",
                                            new List<string>(){ "trout", "potato", "carrot" },
                                            new VerbilizationAction("make", "made")) }
    };

    public static List<string> plantableCrops = new List<string>() {
        "potato",
        "carrot",
        "strawberry",
        "blackberry",
        "flour"
    };


}


