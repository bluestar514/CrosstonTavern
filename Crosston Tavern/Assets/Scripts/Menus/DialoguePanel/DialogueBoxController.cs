﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
    public PlayerTextBoxController playerResponseMenu;
    public SpeakerTagController speakerTagPanel;
    public NPCPortraitController portraitController;

    

    public virtual void Initialize(BarSpaceController bsc)
    {
        playerResponseMenu.Initialize(bsc);
    }

    public virtual void DisplayNPCAction(DialogueUnit dialogueUnit)
    {
        CloseAll();

        speakerTagPanel.Fill(dialogueUnit.speakerName);
        speakerTagPanel.Open();

        portraitController.SetPortrait(dialogueUnit.speakerName, dialogueUnit.emotion);
    }

    public virtual void DisplayPlayerActions(List<DialogueUnit> dialogueUnits)
    {
//        Debug.Log("DialogueBoxController: Displaying Player Options");

        CloseAll();
        speakerTagPanel.Open();

        playerResponseMenu.Open();
        playerResponseMenu.Fill(dialogueUnits);
    }

    void CloseAll()
    {
        speakerTagPanel.Close();
        playerResponseMenu.Close();
    }

}
