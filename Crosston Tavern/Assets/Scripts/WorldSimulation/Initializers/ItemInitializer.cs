using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInitializer 
{
    public enum ItemClass
    {
        food,
        tool,
        misc,
        none
    }

    public static Dictionary<string, FoodItem> menu = new Dictionary<string, FoodItem>() {
        { "strawberry_cake", new FoodItem("strawberry_cake",
                                            new List<string>(){ "strawberry", "egg", "milk" },
                                            new VerbalizationActionCooking("bake", "baked", "strawberry_cake")) },
        { "fried_salmon", new FoodItem("fried_salmon",
                                            new List<string>(){ "salmon", "herb" },
                                            new VerbalizationActionCooking("make", "made", "fried_salmon")) },
        { "blackberry_tart", new FoodItem("blackberry_tart",
                                            new List<string>(){ "blackberry", "flour", "egg" },
                                            new VerbalizationActionCooking("bake", "baked", "blackberry_tart")) },
        { "stewed_trout", new FoodItem("stewed_trout",
                                            new List<string>(){ "trout", "potato", "carrot" },
                                            new VerbalizationActionCooking("make", "made", "stewed_trout")) }
    };

    public static List<string> plantableCrops = new List<string>() {
        "potato",
        "carrot",
        "flour"
    };

    static Dictionary<ItemClass, List<string>> itemsOfType = new Dictionary<ItemClass, List<string>>() {
        {ItemClass.food, new List<string>() {
        
            }
        },
        {ItemClass.tool, new List<string>() {
                "fishing_rod"
            }
        }
    };

    public static Dictionary<ItemClass, List<string>> GetSortedItems()
    {
        Dictionary<ItemClass, List<string>> sortedItems = itemsOfType;
        
        foreach(KeyValuePair<string, FoodItem> pair in menu) {
            List<string> addableItems = new List<string>( pair.Value.ingredients);
            addableItems.Add(pair.Key);

            foreach(string item in addableItems) {
                if (!sortedItems[ItemClass.food].Contains(item)) {
                    sortedItems[ItemClass.food].Add(item);
                }
            }
            
        }

        return sortedItems;
    }

    public static bool IsItem(string item, ItemClass itemClass)
    {
        if (itemClass == ItemClass.misc) {
            return GetItemClass(item) == ItemClass.misc;
        } else if (itemClass == ItemClass.none) {
            return false;
        } else {
            return GetSortedItems()[itemClass].Contains(item);
        }
    }

    public static ItemClass GetItemClass(string item)
    {
        foreach(KeyValuePair<ItemClass, List<string>> pair in GetSortedItems()) {
            if (pair.Value.Contains(item)) return pair.Key;
        }

        return ItemClass.misc;
    }
}


