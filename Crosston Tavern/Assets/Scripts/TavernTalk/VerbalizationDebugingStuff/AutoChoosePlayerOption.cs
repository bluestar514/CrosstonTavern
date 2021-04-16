using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoChoosePlayerOption : DialogueBoxController
{
    BarSpaceController bsc;

    Coroutine activePlayerOption;

    public float choiceDelayTime;

    public override void DisplayPlayerActions(List<DialogueUnit> dialogueUnits)
    {
        base.DisplayPlayerActions(dialogueUnits);

        int rand = Random.Range(0, dialogueUnits.Count);

        activePlayerOption = StartCoroutine(PushInASec(dialogueUnits[rand]));


        //MUST PUT THIS IN A COROUTINE BECAUSE OTHERWISE THIS BECOMES AN INFINITE LOOP!!!!!!!!
        //bsc.PlayerChoiceButtonPush(dialogueUnits[rand]); 
    }

    public override void DisplayNPCAction(DialogueUnit dialogueUnit)
    {
        base.DisplayNPCAction(dialogueUnit);

        if(activePlayerOption != null)  
            StopCoroutine(activePlayerOption);
    }

    public override void Initialize(BarSpaceController bsc)
    {
        base.Initialize(bsc);
        this.bsc = bsc;
    }

    IEnumerator PushInASec(DialogueUnit dialogueUnit)
    {
        yield return new WaitForSeconds(choiceDelayTime);
        bsc.PlayerChoiceButtonPush(dialogueUnit);
    }
}
