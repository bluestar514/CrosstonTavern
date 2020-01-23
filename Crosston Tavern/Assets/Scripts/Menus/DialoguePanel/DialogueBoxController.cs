using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
    public NPCTextBoxController speachPanelMenu;
    public PlayerTextBoxController playerResponseMenu;
    public SpeakerTagController speakerTagPanel;

    Patron patron = new Patron("Marco");
    Patron barkeep = new Patron("Barkeep");


    public void Start()
    {
        playerResponseMenu.Initialize(this);


        DisplayPlayerActions();
        //NPCTakeAction();
    }

    public void OnClick()
    {
        if (speachPanelMenu.gameObject.activeInHierarchy)
        {
            DisplayPlayerActions();
        }
        
    }

    public void PlayerChoiceButtonPush(DialogueUnit dialogueUnit)
    {
        print(dialogueUnit.ToString());
        //Add to log...

        NPCTakeAction();
    }


    void NPCTakeAction()
    {
        DisplayNPCAction(patron.ExpressSocialMove(patron.PickSocialMove()));
    }

    void DisplayNPCAction(DialogueUnit dialogueUnit)
    {
        CloseAll();

        speakerTagPanel.Fill(dialogueUnit.speakerName);
        speakerTagPanel.Open();

        speachPanelMenu.Fill(dialogueUnit.verbalization);
        speachPanelMenu.Open();
    }

    void DisplayPlayerActions()
    {
        CloseAll();
        speakerTagPanel.Fill("Barkeep");
        speakerTagPanel.Open();

        playerResponseMenu.Open();

        List<SocialMove> bestSocialMoves = barkeep.PickBestSocialMoves(4);
        List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
        foreach(SocialMove socialMove in bestSocialMoves)
        {
            dialogueUnits.Add(barkeep.ExpressSocialMove(socialMove));
        }
        playerResponseMenu.Fill(dialogueUnits);
    }

    void CloseAll()
    {
        speakerTagPanel.Close();
        speachPanelMenu.Close();
        playerResponseMenu.Close();
    }

}
