using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChoiceButton : MonoBehaviour
{
    public Text buttonText;

    BarSpaceController supervisor;

    DialogueUnit dialogueUnit;

    public void LabelButton(DialogueUnit content, BarSpaceController bsc)
    {
        buttonText.text = content.verbalization;
        dialogueUnit = content;

        supervisor = bsc;
    }

    public void OnClick()
    {
        supervisor.PlayerChoiceButtonPush(dialogueUnit);
    }
}
