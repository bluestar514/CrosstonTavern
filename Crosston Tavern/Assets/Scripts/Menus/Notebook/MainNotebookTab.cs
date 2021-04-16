using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainNotebookTab : MonoBehaviour
{

    public virtual void UpdateDaily()
    {
        return;
    }

    public virtual bool AddWorldFact(WorldFact fact)
    {
        return false;
    }

    public virtual bool RemoveWorldFact(WorldFact fact)
    {
        return false;
    }

    public virtual void OpenTab()
    {
        gameObject.SetActive(true);
    }

    public virtual void OpenSubTab(string str)
    {

    }
}
