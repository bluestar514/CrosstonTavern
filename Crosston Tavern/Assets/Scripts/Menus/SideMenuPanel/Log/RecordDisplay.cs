using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordDisplay : DisplayPanel<DialogueUnit>
{
    public Text nameTag;
    public VerticalLayoutGroup namePositioner;
    public VerticalLayoutGroup contentPositioner;

    public bool ready = false;

    public override void Fill(DialogueUnit fact)
    {
        this.fact = fact;

        

        nameTag.text = VerbalizationDictionary.Replace( fact.speakerName);


        if(fact.speakerName == "barkeep") {
            factText.text = fact.verbalization;

            namePositioner.childAlignment = TextAnchor.UpperRight;
            contentPositioner.padding.left = 0;
            contentPositioner.padding.right = 50;

            ready = true;
        } else {
            StartCoroutine(TypeWriter(fact.verbalization, .05f));
        }
    }


    IEnumerator TypeWriter(string content, float textSpeed)
    {
        factText.text = "";
        foreach(char c in content) {
            factText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        ready = true;
    }
}
