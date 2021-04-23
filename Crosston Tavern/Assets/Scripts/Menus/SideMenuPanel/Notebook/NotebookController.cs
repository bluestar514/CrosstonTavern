using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookController : SideMenuController<WorldFact>
{
    public MainNotebookController notebook;

    public void UpdateDaily()
    {
        notebook.UpdateDaily();
        Clear();
    }

    public void AddManyElements(List<WorldFact> facts)
    {
        foreach (WorldFact fact in facts) {
            //Debug.Log(fact);
            AddElement(fact);
        }

        notebook.AddMany(facts);
    }

    public override void AddElement(WorldFact element)
    {
        bool addToSideNotebook = notebook.AddWorldFact(element);


        if(addToSideNotebook) base.AddElement(element);
    }

    public void RemoveElement(WorldFact fact)
    {
        DisplayPanel<WorldFact> matchingPanel = null;
        foreach(DisplayPanel<WorldFact> panel in displayPanels) {
            if (panel.Matches(fact)) {
                matchingPanel = panel;
                break;
            }
        }
        if (matchingPanel != null) { 
            displayPanels.Remove(matchingPanel);
            Destroy(matchingPanel.gameObject);
        }

        notebook.RemoveWorldFact(fact);


    }

    void Clear()
    {
        foreach(Transform child in contentPanel.transform) {
            Destroy(child.gameObject);
        }

        displayPanels = new List<DisplayPanel<WorldFact>>();
    }
}
