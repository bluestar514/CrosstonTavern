using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordDisplay : DisplayPanel<DialogueUnit>
{
    public Text nameTag;
    public VerticalLayoutGroup namePositioner;
    public VerticalLayoutGroup contentPositioner;

    float textSpeed = .05f;
    public void Init(float textSpeed)
    {
        this.textSpeed = textSpeed;
    }

    public override IEnumerator Fill(DialogueUnit fact)
    {
        this.fact = fact;

        nameTag.text = VerbalizationDictionary.Replace( fact.speakerName);


        if(fact.speakerName == "barkeep") {
            factText.text = fact.verbalization;

            namePositioner.childAlignment = TextAnchor.UpperRight;
            contentPositioner.childAlignment = TextAnchor.UpperRight;

            yield break;
        } else {
            yield return TypeWriter(fact.verbalization, textSpeed);
        }
    }


    IEnumerator TypeWriter(string content, float textSpeed)
    {

        factText.text = "";
        foreach(char c in content) {
            factText.text += c;

            if (DetectSkip() || textSpeed <= 0) {
                factText.text = content;
                break;
            }


            yield return new WaitForSeconds(textSpeed);
        }

    }

    bool DetectSkip()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) return true;
        else return false;
    }
}
