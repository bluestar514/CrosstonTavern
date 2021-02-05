using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem 
{
    public string name;
    public List<string> ingredients;
    public VerbilizationAction verb;

    public FoodItem(string name, List<string> ingredients, VerbilizationAction verb)
    {
        this.name = name;
        this.ingredients = ingredients;
        this.verb = verb;
    }
} 
