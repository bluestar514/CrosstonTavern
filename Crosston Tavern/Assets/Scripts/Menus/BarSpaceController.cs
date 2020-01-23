using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarSpaceController : MonoBehaviour
{
    public DialogueBoxController dbc;
    public NotebookController nc;

    Patron patron = new Patron("Marco");
    Patron barkeep = new Patron("Barkeep");

    public void Start()
    {
        dbc.Initialize(this);
        NPCPhase();
    }

    public void NPCPhase()
    {
        dbc.DisplayNPCAction(patron.ExpressSocialMove(patron.PickSocialMove()));
    }

    public void PlayerPhase()
    {
        List<SocialMove> bestSocialMoves = barkeep.PickBestSocialMoves(4);
        List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
        foreach (SocialMove socialMove in bestSocialMoves)
        {
            dialogueUnits.Add(barkeep.ExpressSocialMove(socialMove));
        }
        dbc.DisplayPlayerActions(dialogueUnits);
    }

    public void PlayerChoiceButtonPush(DialogueUnit dialogueUnit)
    {
        print(dialogueUnit.ToString());
        //Add to log...

        NPCPhase();
    }

    public void AdvanceNPCDialogue()
    {
        PlayerPhase();
    }

}
