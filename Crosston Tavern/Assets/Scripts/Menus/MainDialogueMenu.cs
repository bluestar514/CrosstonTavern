using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDialogueMenu : MonoBehaviour
{
    public NPCTextBoxController speachPanelMenu;
    public PlayerTextBoxController playerResponseMenu;
    public SpeakerTagController speakerTagPanel;


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
        speachPanelMenu.Fill(socialMove.ToString());
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
