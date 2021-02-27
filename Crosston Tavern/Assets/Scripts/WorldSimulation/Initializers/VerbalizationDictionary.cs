using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerbalizationDictionary 
{

    public enum Form
    {
        singular,
        plural,
        general
    }

    public static Dictionary<string, EntityVerbalization> verbalizationData = new Dictionary<string, EntityVerbalization>() {
        {"strawberry_cake", new EntityVerbalization("strawberry_cake", "strawberry cake", "strawberry cakes") },
        {"fried_salmon", new EntityVerbalization("fried_salmon", "fried salmon", "fried salmon") },
        {"blackberry_tart", new EntityVerbalization("blackberry_tart", "blackberry tart", "blackberry tarts") },
        {"stewed_trout", new EntityVerbalization("stewed_trout", "bowl of stewed trout", "bowls of stewed trout", "stewed trout") },


        {"dandelion", new EntityVerbalization("dandelion", "dandelion", "dandelions") }, 
        {"trout", new EntityVerbalization("trout", "trout") },
        {"salmon", new EntityVerbalization("salmon", "salmon") },
        {"fish_bait", new EntityVerbalization("fish_bait", "fish bait") },
        {"fishing_rod", new EntityVerbalization("fishing_rod", "fishing rod", "fishing rods") },
        {"bug", new EntityVerbalization("bug", "bug", "bugs") },
        {"seed_potato", new EntityVerbalization("seed_potato", "potato seed", "potato seeds") },
        {"potato", new EntityVerbalization("potato", "potato", "potatos") },
        {"seed_carrot", new EntityVerbalization("seed_carrot", "carrot seed", "carrot seeds") },
        {"carrot", new EntityVerbalization("carrot", "carrot", "carrots") },
        {"seed_strawberry", new EntityVerbalization("seed_strawberry", "strawberry seed", "strawberry seeds") },
        {"strawberry", new EntityVerbalization("strawberry", "strawberry", "strawberries") },
        {"seed_blackberry", new EntityVerbalization("seed_blackberry", "blackberry seed", "blackberry seeds") },
        {"blackberry", new EntityVerbalization("blackberry", "blackberry", "blackberries") },
        {"seed_flour", new EntityVerbalization("seed_flour", "wheat seed", "wheat seeds") },
        {"flour", new EntityVerbalization("flour", "bag of flour", "bags of flour", "flour") },
        {"egg", new EntityVerbalization("egg", "egg", "eggs") },
        {"milk", new EntityVerbalization("milk", "carton of milk", "cartons of milk", "milk") },
        {"butter", new EntityVerbalization("butter", "stick of butter", "sticks of butter", "butter") },
        {"tulip", new EntityVerbalization("tulip", "tulip", "tulips") },
        {"rose", new EntityVerbalization("rose", "rose", "roses") },
        {"morning_rose", new EntityVerbalization("morning_rose", "morning rose", "morning roses") },
        {"evening_tulip", new EntityVerbalization("evening_tulip", "evening tulip", "evening tulips") },


        {"alicia", new EntityVerbalization("alicia", "Alicia") },
        {"bob", new EntityVerbalization("bob", "Bob") },
        {"clara", new EntityVerbalization("clara", "Clara") },
        {"dirk", new EntityVerbalization("dirk", "Dirk") },

        {"avery", new EntityVerbalization("avery", "Avery") },
        {"sammy", new EntityVerbalization("sammy", "Sammy") },
        {"finley", new EntityVerbalization("finley", "Finley") },


        {"river_farm", new EntityVerbalization("river_farm", "the river by the farm") },
        {"chicken_farm", new EntityVerbalization("chicken_farm", "chickens") },
        {"cow_farm", new EntityVerbalization("cow_farm", "cows") },
        {"field_farm", new EntityVerbalization("field_farm", "the field") },

        {"river_field", new EntityVerbalization("river_field", "the river running through the meadow") },
        {"meadow_field", new EntityVerbalization("meadow_field", "the meadow") },

        {"kitchen_inn", new EntityVerbalization("kitchen_inn", "the inn") },

        {"tackle_shop_town", new EntityVerbalization("tackle_shop_town", "the tackle shop") },

        {"brush_forest", new EntityVerbalization("brush_forest", "the forest") },
        
        
        
    };

    public static string Replace(string id, Form form = Form.singular)
    {
        if (verbalizationData.ContainsKey(id)) {
            switch (form) {
                case Form.singular:
                    return verbalizationData[id].singularNoun;
                case Form.plural:
                    return verbalizationData[id].pluralNoun;
                case Form.general:
                    return verbalizationData[id].generalNoun;
            }
        }

        return id;
    }


    public static string VerbalizePreferenceLevel(PreferenceLevel level, bool endInS=false)
    {
        string levelStr = level.ToString();
        levelStr = levelStr.Remove(levelStr.Length - 1, 1);

        if(endInS) levelStr += "s";

        return levelStr;
    }
}


public class EntityVerbalization
{
    public string id;
    public string singularNoun;
    public string pluralNoun;
    public string generalNoun;

    public EntityVerbalization(string id, string singularNoun, string pluralNoun="", string generalNoun="")
    {
        this.id = id;
        this.singularNoun = singularNoun;
        if (pluralNoun == "") pluralNoun = singularNoun;
        this.pluralNoun = pluralNoun;

        if (generalNoun == "") generalNoun = singularNoun;
        this.generalNoun = generalNoun;
    }
}