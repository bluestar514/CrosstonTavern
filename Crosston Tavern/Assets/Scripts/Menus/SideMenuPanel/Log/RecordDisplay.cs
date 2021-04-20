using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordDisplay : DisplayPanel<DialogueUnit>
{
    public Text nameTag;
    public VerticalLayoutGroup namePositioner;
    public VerticalLayoutGroup contentPositioner;

    public override IEnumerator Fill(DialogueUnit fact)
    {
        this.fact = fact;

        

        nameTag.text = VerbalizationDictionary.Replace( fact.speakerName);


        if(fact.speakerName == "barkeep") {
            factText.text = fact.verbalization;

            namePositioner.childAlignment = TextAnchor.UpperRight;
            contentPositioner.childAlignment = TextAnchor.UpperRight;
            //contentPositioner.padding.left = 0;
            //contentPositioner.padding.right = 50;

            yield break;
        } else {
            yield return TypeWriter(fact.verbalization, .05f);
        }
    }


    IEnumerator TypeWriter(string content, float textSpeed)
    {
        Debug.Log("RecordDisplay: Starting TypeWriter");

        factText.text = "";
        foreach(char c in content) {
            factText.text += c;

            if (DetectSkip()) {
                Debug.Log("RecordDisplay: Detecting Typewriter skip");
                factText.text = content;
                break;
            }


            yield return new WaitForSeconds(textSpeed);
        }

        Debug.Log("RecordDisplay: Ending TypeWriter");
    }

    bool DetectSkip()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) return true;
        else return false;
    }
}
