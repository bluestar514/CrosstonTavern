using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextBoxController : MenuElement
{
    public List<PlayerChoiceButton> playerChoiceButtons;

    DialogueBoxController mainMenu;

    public void Initialize(DialogueBoxController mm)
    {
        mainMenu = mm;
    }

    public void Fill(List<DialogueUnit> playerChoices)
    {
        int minNum = Mathf.Min(playerChoiceButtons.Count, playerChoices.Count);

        for(int i=0; i<minNum; i++)
        {
            playerChoiceButtons[i].LabelButton(playerChoices[i], this);
        }

    }

    public void SelectChoice(DialogueUnit dialogueUnit)
    {
        mainMenu.PlayerChoiceButtonPush(dialogueUnit);
    }
}
