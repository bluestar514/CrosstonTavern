using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarSpaceController : MonoBehaviour
{
    public DialogueBoxController dbc;
    public NotebookController nc;
    public LogController lc;
    public PatronPicker pp;
    public WorldHub worldHub;

    static List<SocialMove> genericSocialMoves = new List<SocialMove>()
        {
            new SocialMove("talk"),
            new SocialMove("argue"),
            new SocialMove("insult"),
            new SocialMove("compliment"),
            new SocialMove("complain"),
            new SocialMove("brag"),
            new SocialMove("askAbout"),
            new SocialMove("smallTalk"),
            new SocialMove("agree"),
            new SocialMove("disagree"),
            new SocialMove("gossip")
        };

    static List<SocialMove> barkeeperMoves = new List<SocialMove>() {
            new SocialMove("askAboutGoals"),
            new SocialMove("askAboutGoalFrustration"),
            new SocialMove("askAboutDay"),
            new SocialMove("askAboutObservation"),
            new SocialMove("askAboutExcitement"),
            new SocialMove("askAboutDisapointment")
    };


    public Patron patron;
    public Patron barkeep;

    public void Start()
    {
        
        gameObject.SetActive(false);

        dbc.Initialize(this);
        nc.Initialize(new List<Fact>());
        lc.Initialize(new List<string>());
    }

    public void SetPatron(Townie townie)
    {

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeep = new Patron(barkeepTownie, barkeeperMoves);
        patron = new Patron(townie, genericSocialMoves);
        pp.gameObject.SetActive(false);

        PlayerPhase();
    }


    public void NPCPhase(SocialMove prompt)

    {
        DialogueUnit npcDialogue = patron.ExpressSocialMove(patron.PickSocialMove(prompt));

        dbc.DisplayNPCAction(npcDialogue);
        lc.AddElement(npcDialogue.speakerName + ": " + npcDialogue.verbalization);
        AddAllFacts(npcDialogue.facts);
    }

    public void PlayerPhase()
    {
        List<SocialMove> bestSocialMoves = barkeep.GetSocialMoves(patron.townie);
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

        NPCPhase(dialogueUnit.underpinningSocialMove);
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
