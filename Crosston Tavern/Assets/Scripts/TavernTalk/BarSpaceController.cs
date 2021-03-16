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


    PatronEngine patronEngine;
    BarkeepEngine barkeepEngine;
    ConversationVerbalizer patronVerbalizer;
    ConversationVerbalizer barkeepVerbalizer;

    SocialMove lastSocialMove = null;

    List<FoodItem> barMenu = new List<FoodItem>(from food in  ItemInitializer.menu.Values
                                                select food);


    public virtual void Start()
    {
        Init();
    }

    protected  void Init()
    {
        dbc.Initialize(this);
        nc.Initialize(new List<WorldFact>());
        lc.Initialize(new List<DialogueUnit>());

        bps = new BarPatronSelector(worldHub.GetTownies(), worldHub.ws, validPatronNames);

        

        SetNextPatron();
    }


    public void SetNextPatron()
    {

        Townie patron = bps.PickRandomPatron();

        if(patron != null) 
            SetPatron(patron);
        else {
            AdvanceDay();
        }    
    }

    public void AdvanceDay()
    {
        worldHub.DayStep();
        AddDaySeperator();
        bps.NewNight();
        SetNextPatron();
    }

    public void SetPatron(Townie townie)
    {
        AddConvSeperator();

        Person partner = townie.townieInformation;
        bps.Visit(partner.id);

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeepEngine = new BarkeepEngine(barkeepTownie, partner, barMenu);
        patronEngine = new PatronEngine(townie, barMenu); 
        barkeepVerbalizer = new ConversationVerbalizer(barkeepTownie, partner.id);
        patronVerbalizer = new ConversationVerbalizer(townie, "barkeep");

        pp.gameObject.SetActive(false);

        NPCPhase(new SocialMove("start"));

        //PlayerPhase();
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
        
        patronEngine.LearnFromInput(prompt.mentionedFacts);

        lastSocialMove = patronEngine.GiveResponse(prompt);
        patronEngine.DecrementTurns();

        if (lastSocialMove.verb.Contains( "pass" ) || lastSocialMove.verb.Contains("ENDCONVERSATION")) {
            AdvanceNPCDialogue();
        }else { 
            NPCSpeak(lastSocialMove);
        }

        Debug.Log(patronEngine.speaker + ":"+ lastSocialMove + "(" + lastSocialMove.verb + ")");

        PlayerPhase();
    }

    void NPCSpeak(SocialMove move)
    {
        DialogueUnit npcDialogue = patronVerbalizer.ExpressSocialMove(move);

        dbc.DisplayNPCAction(npcDialogue);
        lc.AddElement(npcDialogue);

        List<WorldFact> newFacts = barkeepEngine.LearnFromInput(npcDialogue.facts);
        AddAllFacts(npcDialogue.facts);
        RemoveRetractedFacts(move.retractedFacts);
    }

    public void PlayerPhase()
    {

        if ( patronEngine.MaxTurns <= 0) {
            lastSocialMove = new SocialMove("turnsUp");

            NPCPhase(lastSocialMove);
            
        } else {

            List<SocialMove> bestSocialMoves = barkeepEngine.GetSocialMoves(lastSocialMove);
            List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
            foreach (SocialMove socialMove in bestSocialMoves) {
                dialogueUnits.Add(barkeepVerbalizer.ExpressSocialMove(socialMove));
            }
            dbc.DisplayPlayerActions(dialogueUnits);


            
        }
    }

    public void PlayerChoiceButtonPush(DialogueUnit dialogueUnit)
    {
        Debug.Log("Barkeep: " + dialogueUnit.underpinningSocialMove + "(" + dialogueUnit.underpinningSocialMove.verb + ")");

        lc.AddElement(dialogueUnit);

        if(dialogueUnit.underpinningSocialMove is SocialMenu menu) {
            lastSocialMove = menu;
            PlayerPhase();
        } else {
            barkeepEngine.DoMove(dialogueUnit.underpinningSocialMove);
            NPCPhase(dialogueUnit.underpinningSocialMove);
        }
    }

    public void AdvanceNPCDialogue()
    {
        if (lastSocialMove.verb.Contains("ENDCONVERSATION")) {
            SetNextPatron();
        } else {
            PlayerPhase();
        }
    }


    void AddAllFacts(List<WorldFact> facts)
    {
        nc.AddManyElements(facts);
    }


    void RemoveRetractedFacts(List<WorldFact> facts)
    {
        foreach(WorldFact fact in facts) {
            nc.RemoveElement(fact);
        }
    }
}
