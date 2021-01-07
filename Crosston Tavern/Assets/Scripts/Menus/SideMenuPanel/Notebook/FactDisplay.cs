using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactDisplay : DisplayPanel<WorldFact>
{

    public override void Fill(WorldFact fact)
    {
        base.Fill(fact);
        Verbalizer v = new Verbalizer("barkeep", "none");

        if (fact is WorldFactEvent) {
            WorldFactEvent e = (WorldFactEvent)fact;
            factText.text = v.VerbalizeAction(e.action, false);
        } else {
            factText.text = fact.ToString();
        }

        
    }
}
