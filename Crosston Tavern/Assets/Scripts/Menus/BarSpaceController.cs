using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarSpaceController : MonoBehaviour
{
    public DialogueBoxController dbc;
    public NotebookController nc;
    public LogController lc;

    Patron patron = new Patron("Marco");
    Patron barkeep = new Patron("Barkeep");

    public void Start()
    {
        dbc.Initialize(this);
        nc.Initialize(new List<Fact>());
        lc.Initialize(new List<string>());

        NPCPhase();
    }

    public void NPCPhase()
    {
        DialogueUnit npcDialogue = patron.ExpressSocialMove(patron.PickSocialMove());

        dbc.DisplayNPCAction(npcDialogue);
        lc.AddElement(npcDialogue.speakerName + ": " + npcDialogue.verbalization);
        AddAllFacts(npcDialogue.facts);
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
        lc.AddElement(dialogueUnit.speakerName + ": " + dialogueUnit.verbalization);

        NPCPhase();
    }

    public void AdvanceNPCDialogue()
    {
        PlayerPhase();
    }


    void AddAllFacts(List<Fact> facts)
    {
        foreach(Fact fact in facts) {
            nc.AddElement(fact);
        }
    }

}
