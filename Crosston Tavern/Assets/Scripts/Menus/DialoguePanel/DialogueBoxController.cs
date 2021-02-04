using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
    public NPCTextBoxController speachPanelMenu;
    public PlayerTextBoxController playerResponseMenu;
    public SpeakerTagController speakerTagPanel;
    public NPCPortraitController portraitController;

    

    public void Initialize(BarSpaceController bsc)
    {
        playerResponseMenu.Initialize(bsc);
    }

    public void DisplayNPCAction(DialogueUnit dialogueUnit)
    {
        CloseAll();

        speakerTagPanel.Fill(dialogueUnit.speakerName);
        speakerTagPanel.Open();

        speachPanelMenu.Fill(dialogueUnit.verbalization);
        speachPanelMenu.Open();

        portraitController.SetPortrait(dialogueUnit.speakerName, dialogueUnit.emotion);
    }

    public void DisplayPlayerActions(List<DialogueUnit> dialogueUnits)
    {
        CloseAll();
        speakerTagPanel.Fill("Barkeep");
        speakerTagPanel.Open();

        playerResponseMenu.Open();
        playerResponseMenu.Fill(dialogueUnits);
    }

    void CloseAll()
    {
        speakerTagPanel.Close();
        speachPanelMenu.Close();
        playerResponseMenu.Close();
    }

}
