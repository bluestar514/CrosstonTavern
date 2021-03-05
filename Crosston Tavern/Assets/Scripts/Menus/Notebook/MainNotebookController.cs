﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainNotebookController : MonoBehaviour
{
    public List<MainNotebookTab> tabs;
    public NotebookLocationsPanel locationsTab;

    public void OpenTab(GameObject openTab)
    {
        foreach(MainNotebookTab tab in tabs) {
            tab.gameObject.SetActive(false);
        }

        openTab.SetActive(true);
    }

    public void AddMany(List<WorldFact> facts)
    {
        foreach(MainNotebookTab tab in tabs) {
            if(tab is NotebookGoalsPanel goalsPanel) {
                goalsPanel.AddMany(facts);
            }
        }
    }

    public bool AddWorldFact(WorldFact fact)
    {
        bool addedSuccessfully = false;
        foreach (MainNotebookTab tab in tabs) {
            bool addedThisTab = tab.AddWorldFact(fact);

            addedSuccessfully = addedThisTab || addedSuccessfully;
        }

        return addedSuccessfully;
    }

    public bool RemoveWorldFact(WorldFact fact)
    {
        bool removedSuccessfully = false;
        foreach (MainNotebookTab tab in tabs) {
            bool removedThisTab = tab.RemoveWorldFact(fact);

            removedSuccessfully = removedThisTab || removedSuccessfully;
        }

        return removedSuccessfully;
    }
}
