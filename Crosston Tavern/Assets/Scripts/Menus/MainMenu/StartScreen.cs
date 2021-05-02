using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject World;
    public GameObject BarCanvas;
    public BarSpaceController mainController;
    public BarSpaceControllerDebug debugController;


    public void StartDemo()
    {
        mainController.enabled = true;
        debugController.enabled = false;
        WakeEverything();

        mainController.StartSetup();

        MainMenuCanvas.SetActive(false);
    }

    public void StartAuto()
    {
        mainController.enabled = false;
        debugController.enabled = true;
        WakeEverything();

        debugController.StartSetup();

        MainMenuCanvas.SetActive(false);
    }


    void WakeEverything()
    {
        World.SetActive(true);
        BarCanvas.SetActive(true);

        World.GetComponent<WorldHub>().StartSetup();
    }
}
