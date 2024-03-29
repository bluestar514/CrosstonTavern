﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarSpaceController : MonoBehaviour
{
    
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

    static SocialMove convStart = new SocialMove("start");



    public virtual void StartSetup()
    {
        Debug.Log("BarSpaceController: Starting");
        StartCoroutine(WaitUntilDayLoaded(Init));
    }

    protected bool Init()
    {
        Debug.Log("BarSpaceController: Initialization");
        sideNotebookController.Initialize(new List<WorldFact>());
        logController.Initialize(this);

        barPatronSelector = new BarPatronSelector(worldHub.GetTownies(), worldHub.ws, validPatronNames);

        NextConversation();

        return true;
    }

    public void ButtonPushNextPatron()
    {
        Debug.Log("Pressing Next Patron");
        NextConversation();
    }

    public void ButtonPushNextDay()
    {
        Debug.Log("Pressing Next Day");
        AdvanceDay();
    }

    public void ButtonPushPlayerChoice(DialogueUnit dialogueUnit)
    {
        PrintResponse("barkeep", dialogueUnit.underpinningSocialMove);

        StartCoroutine(WaitUntilDisplayed(dialogueUnit, PlayerChoice));
        
    }

    bool PlayerChoice(SocialMove dialogueUnit)
    {
        if (dialogueUnit is SocialMenu menu) {
            Debug.Log("BarSpaceController: Detecting Additional Choice, looping back to player turn");
            lastSocialMove = menu;
            PlayerPhase(dialogueUnit);
        } else {
            Debug.Log("BarSpaceController: Logging player choice in barkeepEngine");
            barkeepEngine.DoMove(dialogueUnit);
            NPCPhase(dialogueUnit);
        }

        return true;
    }

    void NextConversation()
    {
        Townie patron = barPatronSelector.PickRandomPatron();

        //check if there is a patron left
        if(patron != null) {
            SetPatron(patron);
        } else {
            //the day must be over so go start another day
            AdvanceDay();
        }
    }

    public void SetPatron(Townie townie)
    {
        Debug.Log("Setting patron");

        AddConvSeperator();

        Person partner = townie.townieInformation;
        barPatronSelector.Visit(partner.id);

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeepEngine = new BarkeepEngine(barkeepTownie, partner, barMenu);
        patronEngine = new PatronEngine(townie, barMenu);
        barkeepVerbalizer = new ConversationVerbalizer(barkeepTownie, partner.id);
        patronVerbalizer = new ConversationVerbalizer(townie, "barkeep");

        patronPicker.gameObject.SetActive(false);

        NPCPhase(convStart);
    }

    void AdvanceDay()
    {
        worldHub.DayStep();
        StartCoroutine(WaitUntilDayLoaded(StartNewDay));

        logController.ClearLog();

        //should run:
        // - dayloadiner.LoadingScreenAscyn
        // - StartNewDay()
        //      - SetPatron(nextPatron)
        //          -NPCPhase(convStart)
    }

    bool StartNewDay()
    {
        AddDaySeperator();
        barPatronSelector.NewNight();
        sideNotebookController.UpdateDaily();

        Townie patron = barPatronSelector.PickRandomPatron();
        SetPatron(patron);
        return true;
    }



    bool PlayerPhase(SocialMove prompt)
    {
        if (patronEngine.MaxTurns <= 0) {
            Debug.Log("BarSpaceController: Patron out of turns, moving to end of conversation");

            prompt = new SocialMove("turnsUp");

            NPCPhase(prompt);

        } else {
            Debug.Log("BarSpaceController: Opening player options");

            List<SocialMove> bestSocialMoves = barkeepEngine.GetSocialMoves(prompt);
            List<DialogueUnit> dialogueUnits = new List<DialogueUnit>();
            foreach (SocialMove socialMove in bestSocialMoves) {
                dialogueUnits.Add(barkeepVerbalizer.ExpressSocialMove(socialMove));
            }
            logController.DisplayPlayerActions(dialogueUnits);

        }

        return true;
    }

    void NPCPhase(SocialMove prompt)
    {
        patronEngine.LearnFromInput(prompt.mentionedFacts, prompt.retractedFacts);

        SocialMove response = patronEngine.GiveResponse(prompt);
        patronEngine.DecrementTurns();

        PrintResponse(patronEngine.speaker.Id, response);

        if (response.verb.Contains("pass")) {
            Debug.Log("BarSpaceController: Detecting a pass, handing controll back to player");
            PlayerPhase(response);

        } else if (response.verb.Contains("ENDCONVERSATION")) {
            Debug.Log("BarSpaceController: Detecting a Conversation end");

            NextConversation();

        } else {
            
            List<WorldFact> newFacts = barkeepEngine.LearnFromInput(response.mentionedFacts, response.retractedFacts);
            AddAllFacts(newFacts);
            RemoveRetractedFacts(response.retractedFacts);

            Debug.Log("BarSpaceController: NPC(" + patronEngine.speaker + ") responding with " + response);
            DialogueUnit npcDialogue = patronVerbalizer.ExpressSocialMove(response);

            PrintResponse(patronEngine.speaker.Id, npcDialogue.underpinningSocialMove);
            StartCoroutine(DisplayNPCText(npcDialogue));
        }
    }

    IEnumerator DisplayNPCText(DialogueUnit npcDialogue)
    {
        if (npcDialogue is CompoundDialogueUnit compoundResponse) {
            foreach(DialogueUnit unit in compoundResponse.components) {
                logController.DisplayNPCAction(unit);

                Debug.Log("BarSpaceController: NPC(" + patronEngine.speaker + ") done speaking: " + unit);

                yield return logController.AddElement(unit);
            }

            PlayerPhase(compoundResponse.components.Last().underpinningSocialMove);
        } else {
            logController.DisplayNPCAction(npcDialogue);

            Debug.Log("BarSpaceController: NPC(" + patronEngine.speaker + ") done speaking: " + npcDialogue);

            yield return logController.AddElement(npcDialogue);

            PlayerPhase(npcDialogue.underpinningSocialMove);
        }
    }

    IEnumerator WaitUntilDayLoaded(System.Func<bool> toDoAfterDayLoad)
    {
        yield return dayLoader.LoadingScreenAscyn();
        yield return new WaitForEndOfFrame();

        Debug.Log("finished loading day");

        toDoAfterDayLoad();
    }


    IEnumerator WaitUntilDisplayed(DialogueUnit npcDialogue, System.Func<SocialMove, bool> toDoAfterDisplay)
    {
        yield return logController.AddElement(npcDialogue);

        toDoAfterDisplay(npcDialogue.underpinningSocialMove);
    }


    void PrintResponse(string speaker, SocialMove move)
    {
        if (move == null) return;
        if (move.mentionedFacts == null) return;
        if (move.retractedFacts == null) return;

        Debug.Log(speaker + ":" + move +
            "(" + move.verb + ") + [" + string.Join(",", move.mentionedFacts) + "] " +
            " - [" + string.Join(",", move.retractedFacts) + "]");
    }


    public void AddDaySeperator()
    {
        logController.AddDaySeperator(worldHub.ws.Time);
    }

    public void AddConvSeperator()
    {
        logController.AddConversationSeperator();
    }

    void AddAllFacts(List<WorldFact> facts)
    {
        sideNotebookController.AddManyElements(facts);
    }


    void RemoveRetractedFacts(List<WorldFact> facts)
    {
        foreach (WorldFact fact in facts) {
            sideNotebookController.RemoveElement(fact);
        }
    }
}
