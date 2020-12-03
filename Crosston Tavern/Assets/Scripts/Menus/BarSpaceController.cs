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
            //new SocialMove("askAboutGoalFrustration"),
            //new SocialMove("askAboutDayFull"),
            new SocialMove("askAboutDayHighlights"),
            new SocialMove("askAboutObservation"),
            new SocialMove("askAboutExcitement"),
            new SocialMove("askAboutDisapointment")
    };


    //public Patron patron;
    //public Patron barkeep;

    public ConversationController patron;
    public ConversationController barkeep;

    public void Start()
    {
        
        gameObject.SetActive(false);

        dbc.Initialize(this);
        nc.Initialize(new List<WorldFact>());
        lc.Initialize(new List<DialogueUnit>());
    }

    public void SetPatron(Townie townie)
    {

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeep = new ConversationController(barkeepTownie, barkeeperMoves, townie.townieInformation.id); //new Patron(barkeepTownie, barkeeperMoves);
        patron = new ConversationController(townie, genericSocialMoves, "barkeep"); //new Patron(townie, genericSocialMoves);
        pp.gameObject.SetActive(false);

        PlayerPhase();
    }


    public void NPCPhase(SocialMove prompt)

    {
        DialogueUnit npcDialogue = patron.ExpressSocialMove(patron.GiveResponse(prompt));
        patron.LearnFromInput(prompt);

        dbc.DisplayNPCAction(npcDialogue);
        lc.AddElement(npcDialogue);
        AddAllFacts(npcDialogue.facts);
        barkeep.LearnFromInput(npcDialogue.underpinningSocialMove);
    }

    public void PlayerPhase()
    {
        List<SocialMove> bestSocialMoves = barkeep.GetSocialMoves();
        List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
        foreach (SocialMove socialMove in bestSocialMoves)
        {
            dialogueUnits.Add(barkeep.ExpressSocialMove(socialMove));
        }
        dbc.DisplayPlayerActions(dialogueUnits);
    }

    public void PlayerChoiceButtonPush(DialogueUnit dialogueUnit)
    {
        lc.AddElement(dialogueUnit);

        NPCPhase(dialogueUnit.underpinningSocialMove);
    }

    public void AdvanceNPCDialogue()
    {
        PlayerPhase();
    }


    void AddAllFacts(List<WorldFact> facts)
    {
        foreach(WorldFact fact in facts) {
            nc.AddElement(fact);
        }
    }

}
