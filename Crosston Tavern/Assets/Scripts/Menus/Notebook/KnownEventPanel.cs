using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnownEventPanel : MonoBehaviour
{
    public Text text;

    public void Initiate(WorldFactEvent e)
    {
        Verbalizer v = new Verbalizer("barkeep", "none");

        this.text.text = v.VerbalizeAction(e.action, false);
    }
}
