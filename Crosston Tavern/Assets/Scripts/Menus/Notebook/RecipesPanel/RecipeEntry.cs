using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntry : MonoBehaviour
{
    public Text title;
    public Text ingredients;

    public void Set(FoodItem item)
    {
        Set(item.name, item.ingredients);
    }
    public void Set(string title, List<string> ingredients) {
        this.title.text = VerbalizationDictionary.Replace(title, VerbalizationDictionary.Form.general);
        this.ingredients.text = "- " + string.Join("\n- ", from ingredient in ingredients
                                                           select VerbalizationDictionary.Replace(ingredient, VerbalizationDictionary.Form.general));
    }
}
