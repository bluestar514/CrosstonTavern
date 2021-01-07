using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookController : SideMenuController<WorldFact>
{
    public MainNotebookPanel notebook;

    public override void AddElement(WorldFact element)
    {
        base.AddElement(element);

        if (element is WorldFactEvent) {
            WorldFactEvent e = (WorldFactEvent)element;

            notebook.AddEvent(e);
        }
    }
}
