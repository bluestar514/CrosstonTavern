using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookController : SideMenuController<WorldFact>
{
    public MainNotebookPanel notebook;

    public override void AddElement(WorldFact element)
    {
        bool addToSideNotebook = true;

        if (element is WorldFactEvent) {
            WorldFactEvent e = (WorldFactEvent)element;

            addToSideNotebook = notebook.AddEvent(e);
        }


        if(addToSideNotebook) base.AddElement(element);
    }
}
