using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDialogueMenu : MonoBehaviour
{
    public NPCSpeachPanelMenu speachPanelMenu;
    public PlayerResponsePanelMenu playerResponseMenu;
    public SpeakerTagPanelController speakerTagPanel;


    public List<SocialMove> NPCActions;


    public void Start()
    {

        DisplayNPCAction(PickNPCAction());
    }

    public void OnClick()
    {
        DisplayNPCAction(PickNPCAction());
    }



    void DisplayNPCAction(SocialMove socialMove)
    {
        CloseAll();
        speachPanelMenu.Fill(socialMove);
        speachPanelMenu.Open();
    }

    void CloseAll()
    {
        speachPanelMenu.Close();
        playerResponseMenu.Close();
    }


    SocialMove PickNPCAction()
    {
        return NPCActions[Mathf.FloorToInt(Random.value * NPCActions.Count)];
    }
}
