using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextBoxController : MenuElement
{
    public List<PlayerChoiceButton> playerChoiceButtons;

    BarSpaceController mainMenu;

    public void Initialize(BarSpaceController mm)
    {
        mainMenu = mm;
    }

    public void Fill(List<DialogueUnit> playerChoices)
    {
        int minNum = Mathf.Min(playerChoiceButtons.Count, playerChoices.Count);

        for(int i=0; i<minNum; i++)
        {
            playerChoiceButtons[i].LabelButton(playerChoices[i], mainMenu);
        }

    }
}
