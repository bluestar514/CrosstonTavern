using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterObj : MonoBehaviour
{
    public Toggle toggle;
    public Text nameText;

    MainNotebookPanel main;

    public void Initialize(string characterName, MainNotebookPanel main)
    {
        nameText.text = characterName;
        this.main = main;
    }

    public bool GetToggleState()
    {
        return toggle.isOn;
    }

    public void UpdateFilters()
    {
        main.UpdateFilters();
    }
   
}
