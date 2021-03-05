using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookController : SideMenuController<WorldFact>
{
    public MainNotebookController notebook;

    public void AddManyElements(List<WorldFact> facts)
    {
        notebook.AddMany(facts);

        foreach (WorldFact fact in facts) {
            AddElement(fact);
        }
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
}
