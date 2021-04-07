using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarSpaceController : MonoBehaviour
{
    public DialogueBoxController dialogueBoxController;
    public NotebookController sideNotebookController;
    public LogController logController;
    public PatronPicker patronPicker;
    public WorldHub worldHub;
    public BarPatronSelector barPatronSelector;
    public DayLoading dayLoader;


    public List<string> validPatronNames = new List<string>();


    PatronEngine patronEngine;
    BarkeepEngine barkeepEngine;
    ConversationVerbalizer patronVerbalizer;
    ConversationVerbalizer barkeepVerbalizer;

    SocialMove lastSocialMove = null;

    List<FoodItem> barMenu = new List<FoodItem>(from food in  ItemInitializer.menu.Values
                                                select food);


    public virtual void Start()
    {
//        Debug.Log("BarSpaceController: Starting");
        StartCoroutine(WaitForDayLoad());
    }

    IEnumerator WaitForDayLoad()
    {
        yield return dayLoader.LoadNextDay();

        Init();
    }

    protected void Init()
    {
        Debug.Log("BarSpaceController: Initialization");
        dialogueBoxController.Initialize(this);
        sideNotebookController.Initialize(new List<WorldFact>());
        logController.Initialize(new List<DialogueUnit>());

        barPatronSelector = new BarPatronSelector(worldHub.GetTownies(), worldHub.ws, validPatronNames);

        SetNextPatron();
    }


    public void SetNextPatron()
    {
//        Debug.Log("BarSpaceController: Set Next Patron");
        Townie patron = barPatronSelector.PickRandomPatron();

        if(patron != null) 
            SetPatron(patron);
        else {
            StartCoroutine(AdvanceDay());
        }    
    }

    public void ButtonPushAdvanceDay()
    {
        StartCoroutine(AdvanceDay());
    }

    public IEnumerator AdvanceDay()
    {
        worldHub.DayStep();
        yield return StartCoroutine(dayLoader.LoadingScreenAscyn());

//        Debug.Log("BarSpaceController: Begining Evening");
        AddDaySeperator();
        barPatronSelector.NewNight();
        SetNextPatron();
    }

    public void SetPatron(Townie townie)
    {
        AddConvSeperator();

        Person partner = townie.townieInformation;
        barPatronSelector.Visit(partner.id);

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeepEngine = new BarkeepEngine(barkeepTownie, partner, barMenu);
        patronEngine = new PatronEngine(townie, barMenu); 
        barkeepVerbalizer = new ConversationVerbalizer(barkeepTownie, partner.id);
        patronVerbalizer = new ConversationVerbalizer(townie, "barkeep");

        patronPicker.gameObject.SetActive(false);

//        Debug.Log("BarSpaceController: Set "+townie.Id);

        NPCPhase(new SocialMove("start"));

        //PlayerPhase();
    }

    public void AddDaySeperator()
    {
        logController.AddDaySeperator(worldHub.ws.Time);
    }

    public void AddConvSeperator()
    {
        logController.AddConversationSeperator();
    }

    public void NPCPhase(SocialMove prompt)
    {
//        Debug.Log("BarSpaceController: NPC reacting to "+prompt);
        patronEngine.LearnFromInput(prompt.mentionedFacts, prompt.retractedFacts);

        lastSocialMove = patronEngine.GiveResponse(prompt);
        patronEngine.DecrementTurns();

        if (lastSocialMove.verb.Contains("pass")) {
//           Debug.Log("BarSpaceController: Detecting a pass, handing controll back to player");
            PlayerPhase();

        } else if (lastSocialMove.verb.Contains("ENDCONVERSATION")) {
//            Debug.Log("BarSpaceController: Detecting a Conversation end");
            
            SetNextPatron();
            
        } else {
            StartCoroutine( NPCSpeak(lastSocialMove) );
            
        }

        PrintResponse(patronEngine.speaker.Id);

    }

    IEnumerator NPCSpeak(SocialMove move)
    {
        DialogueUnit npcDialogue = patronVerbalizer.ExpressSocialMove(move);

        dialogueBoxController.DisplayNPCAction(npcDialogue);
        yield return logController.AddElement(npcDialogue);

        List<WorldFact> newFacts = barkeepEngine.LearnFromInput(npcDialogue.facts, move.retractedFacts);
        AddAllFacts(npcDialogue.facts);
        RemoveRetractedFacts(move.retractedFacts);

        PlayerPhase();
    }

    public void PlayerPhase()
    {

        if ( patronEngine.MaxTurns <= 0) {
//            Debug.Log("BarSpaceController: Patron out of turns, moving to end of conversation");

            lastSocialMove = new SocialMove("turnsUp");

            NPCPhase(lastSocialMove);
            
        } else {
//            Debug.Log("BarSpaceController: Opening player options");

            List<SocialMove> bestSocialMoves = barkeepEngine.GetSocialMoves(lastSocialMove);
            List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
            foreach (SocialMove socialMove in bestSocialMoves) {
                dialogueUnits.Add(barkeepVerbalizer.ExpressSocialMove(socialMove));
            }
            dialogueBoxController.DisplayPlayerActions(dialogueUnits);


            
        }
    }

    public IEnumerator PlayerChoiceButtonPush(DialogueUnit dialogueUnit)
    {
        //        Debug.Log("BarSpaceController: player picked:"+dialogueUnit);
        //Debug.Log("Barkeep: " + dialogueUnit.underpinningSocialMove + "(" + dialogueUnit.underpinningSocialMove.verb + ")");
        PrintResponse("barkeep");


        yield return logController.AddElement(dialogueUnit);

        if(dialogueUnit.underpinningSocialMove is SocialMenu menu) {
//            Debug.Log("BarSpaceController: Detecting Additional Choice, looping back to player turn");
            lastSocialMove = menu;
            PlayerPhase();
        } else {
//            Debug.Log("BarSpaceController: Logging player choice in barkeepEngine");
            barkeepEngine.DoMove(dialogueUnit.underpinningSocialMove);
            NPCPhase(dialogueUnit.underpinningSocialMove);
        }
    }


    void AddAllFacts(List<WorldFact> facts)
    {
        sideNotebookController.AddManyElements(facts);
    }


    void RemoveRetractedFacts(List<WorldFact> facts)
    {
        foreach(WorldFact fact in facts) {
            sideNotebookController.RemoveElement(fact);
        }
    }


    void PrintResponse(string speaker)
    {
        Debug.Log(speaker + ":" + lastSocialMove +
            "(" + lastSocialMove.verb + ") + [" + string.Join(",", lastSocialMove.mentionedFacts) + "] " +
            " - [" + string.Join(",", lastSocialMove.retractedFacts) + "]");
    }
}
