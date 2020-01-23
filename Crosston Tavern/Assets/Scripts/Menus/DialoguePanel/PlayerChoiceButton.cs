using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChoiceButton : MonoBehaviour
{
    public Text buttonText;

    PlayerTextBoxController supervisor;

    DialogueUnit dialogueUnit;

    public void LabelButton(DialogueUnit content, PlayerTextBoxController ptbc)
    {
        buttonText.text = content.verbalization;
        dialogueUnit = content;

        supervisor = ptbc;
    }

    public void OnClick()
    {
        supervisor.SelectChoice(dialogueUnit);
    }
}
