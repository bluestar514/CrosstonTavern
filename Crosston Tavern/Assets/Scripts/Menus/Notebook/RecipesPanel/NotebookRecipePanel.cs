using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookRecipePanel : MainNotebookTab
{
    public GameObject recipeEntryPanelPrefab;
    public Transform recipeHolder;

    private void Awake()
    {
        foreach (FoodItem food in ItemInitializer.menu.Values) {
            GameObject obj = Instantiate(recipeEntryPanelPrefab, recipeHolder);

            obj.GetComponent<RecipeEntry>().Set(food);
        }
    }

}
