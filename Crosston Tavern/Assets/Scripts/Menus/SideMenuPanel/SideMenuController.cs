using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMenuController<T> : ToggleableMenuElement
{
    public GameObject contentPanel;
    public GameObject p_DisplayPanel;

    List<DisplayPanel<T>> displayPanels = new List<DisplayPanel<T>>();

    public void Initialize(List<T> records)
    {
        GenerateFactPanels(records);
    }

    void GenerateFactPanels(List<T> knownRecords)
    {
        foreach(T record in knownRecords) {
            AddElement(record);
        }
    }

    public virtual void AddElement(T element)
    {
        DisplayPanel<T> panel = Instantiate(p_DisplayPanel, contentPanel.transform).GetComponent<DisplayPanel<T>>();

        panel.Fill(element);

        displayPanels.Add(panel);
    }

}
