﻿using System.Collections;
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


    //public Patron patron;
    //public Patron barkeep;

    ConversationEngine patronEngine;
    ConversationEngine barkeepEngine;
    ConversationVerbalizer patronVerbalizer;
    ConversationVerbalizer barkeepVerbalizer;

    SocialMove lastSocialMove = null;

    List<FoodItem> barMenu = new List<FoodItem>() {
        new FoodItem("strawberry_cake", new List<string>(){"strawberry"}),
        new FoodItem("trout_stew", new List<string>(){"trout"}),
        new FoodItem("fried_salmon", new List<string>(){"salmon" })
    };

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
        if (patronEngine != null) previousPatron = patronEngine.speaker.name;

        SetPatron(bps.PickRandomPatron(previousPatron));
    }
    public void SetPatron(Townie townie)
    {
        string partner = townie.townieInformation.id;

        Townie barkeepTownie = worldHub.GetTownies().Single(x => x.townieInformation.id == "barkeep");
        barkeepEngine = new ConversationEngine(barkeepTownie, partner, barMenu); //new Patron(barkeepTownie, barkeeperMoves);
        patronEngine = new ConversationEngine(townie, "barkeep", barMenu); //new Patron(townie, genericSocialMoves);
        barkeepVerbalizer = new ConversationVerbalizer(barkeepTownie, partner);
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
        
        patronEngine.LearnFromInput(prompt);

        lastSocialMove = patronEngine.GiveResponse(prompt);
        DialogueUnit npcDialogue = patronVerbalizer.ExpressSocialMove(lastSocialMove);
        
        dbc.DisplayNPCAction(npcDialogue);
        lc.AddElement(npcDialogue);
        AddAllFacts(npcDialogue.facts);

        barkeepEngine.LearnFromInput(lastSocialMove);
    }

    public void PlayerPhase()
    {
        Debug.Log(lastSocialMove);

        List<SocialMove> bestSocialMoves = barkeepEngine.GetSocialMoves(lastSocialMove);
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
