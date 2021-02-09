using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordDisplay : DisplayPanel<DialogueUnit>
{
    public Text nameTag;
    public VerticalLayoutGroup namePositioner;
    public VerticalLayoutGroup contentPositioner;

    public override void Fill(DialogueUnit fact)
    {
        this.fact = fact;

        factText.text = fact.verbalization;
        nameTag.text = VerbalizationDictionary.Replace( fact.speakerName);


        if(fact.speakerName == "barkeep") {
            namePositioner.childAlignment = TextAnchor.UpperRight;
            contentPositioner.padding.left = 0;
            contentPositioner.padding.right = 50;
        }
    }
}
