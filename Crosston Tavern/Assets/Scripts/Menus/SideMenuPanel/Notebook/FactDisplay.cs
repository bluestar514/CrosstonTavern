using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactDisplay : DisplayPanel<WorldFact>
{

    public override void Fill(WorldFact fact)
    {
        base.Fill(fact);

        factText.text = fact.Verbalize("barkeeper", "none");
    }
}
