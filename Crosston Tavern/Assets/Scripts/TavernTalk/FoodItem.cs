using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem 
{
    public string name;
    public List<string> ingredients;

    public FoodItem(string name, List<string> ingredients)
    {
        this.name = name;
        this.ingredients = ingredients;
    }
} 
