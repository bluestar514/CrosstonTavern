using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChoiceButton : MonoBehaviour
{
    public Text buttonText;

    BarSpaceController supervisor;

    public DialogueUnit dialogueUnit;

    public void LabelButton(DialogueUnit content, BarSpaceController bsc, int textSize)
    {
        buttonText.text = content.verbalization;
        buttonText.fontSize = textSize;
        dialogueUnit = content;

        supervisor = bsc;
    }

    public void OnClick()
    {
        supervisor.ButtonPushPlayerChoice(dialogueUnit);
    }
}
