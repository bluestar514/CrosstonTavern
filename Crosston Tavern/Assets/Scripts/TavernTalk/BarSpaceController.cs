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
    public BarPatronSelector bps;

    public List<string> validPatronNames = new List<string>();

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
            //new SocialMove("askAboutObservation"),
            new SocialMove("askAboutExcitement"),
            new SocialMove("askAboutDisapointment")
    };


    //public Patron patron;
    //public Patron barkeep;

    ConversationEngine patronEngine;
    ConversationEngine barkeepEngine;
    ConversationVerbalizer patronVerbalizer;
    ConversationVerbalizer barkeepVerbalizer;

    public void Start()
    {
        dbc.Initialize(this);
        nc.Initialize(new List<WorldFact>());
        lc.Initialize(new List<DialogueUnit>());

        bps = new BarPatronSelector(worldHub.GetTownies(), worldHub.ws, validPatronNames);
        SetNextPatron();
    }


    public void SetNextPatron()
    {
        string previousPatron = "";
        if (patronEngine != null) previousPatron = patronEngine.townie.name;

        SetPatron(bps.PickRandomPatron(previousPatron));
    }
    public void SetPatron(Townie townie)
    {
        string partner = townie.townieInformation.id;

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeepEngine = new ConversationEngine(barkeepTownie, barkeeperMoves, partner); //new Patron(barkeepTownie, barkeeperMoves);
        patronEngine = new ConversationEngine(townie, genericSocialMoves, "barkeep"); //new Patron(townie, genericSocialMoves);
        barkeepVerbalizer = new ConversationVerbalizer(barkeepTownie, partner);
        patronVerbalizer = new ConversationVerbalizer(townie, "barkeep");

        pp.gameObject.SetActive(false);

        PlayerPhase();
    }

    public void AddDaySeperator()
    {
        lc.AddDaySeperator(worldHub.ws.Time);
    }

    public void AddConvSeperator()
    {
        lc.AddConversationSeperator();
    }

    public void NPCPhase(SocialMove prompt)

    {
        DialogueUnit npcDialogue = patronVerbalizer.ExpressSocialMove(patronEngine.GiveResponse(prompt));
        patronEngine.LearnFromInput(prompt);

        dbc.DisplayNPCAction(npcDialogue);
        lc.AddElement(npcDialogue);
        AddAllFacts(npcDialogue.facts);
        barkeepEngine.LearnFromInput(npcDialogue.underpinningSocialMove);
    }

    public void PlayerPhase()
    {
        List<SocialMove> bestSocialMoves = barkeepEngine.GetSocialMoves();
        List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
        foreach (SocialMove socialMove in bestSocialMoves)
        {
            dialogueUnits.Add(barkeepVerbalizer.ExpressSocialMove(socialMove));
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
