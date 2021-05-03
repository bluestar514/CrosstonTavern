using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactDisplay : DisplayPanel<WorldFact>
{

    public override IEnumerator Fill(WorldFact fact)
    {
        yield return base.Fill(fact);

        string speaker = "barkeeper";
        string listener = "none";


        factText.text = fact.Verbalize(speaker, listener);
    }
}
