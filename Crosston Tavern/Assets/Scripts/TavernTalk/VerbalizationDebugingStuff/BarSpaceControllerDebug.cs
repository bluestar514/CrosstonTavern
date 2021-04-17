 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarSpaceControllerDebug : BarSpaceController
{
    public BarSpaceController controllerOverride;

    public WriteConversation externalLogger;
    public AutoChoosePlayerOption autoPlayer;


    public override void Start()
    {
        InitialSettings();

        
        StartCoroutine(DoWhenReady());
    }

    IEnumerator DoWhenReady()
    {
        yield return dayLoader.LoadingScreenAscyn();
        Init();

    }

    void InitialSettings()
    {
        //if (dialogueboxcontroller != null)
        //    debug.logerror("dialogueboxcontroller in barspacecontrollerdebugger should not be set" +
        //        " as it is overriden with the autoplayer setting");
        //dialogueboxcontroller = autoplayer;

        if (sideNotebookController != null)
            Debug.LogError("sideNotebookController in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the controllerOverride setting");
        sideNotebookController = controllerOverride.sideNotebookController;

        if (logController != null)
            Debug.LogError("logController in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the externalLogger setting");
        logController = externalLogger;

        if (patronPicker != null)
            Debug.LogError("patronPicker in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the controllerOverride setting");
        patronPicker = controllerOverride.patronPicker;

        if (worldHub != null)
            Debug.LogError("worldHub in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the controllerOverride setting");
        worldHub = controllerOverride.worldHub;

        if (barPatronSelector != null)
            Debug.LogError("barPatronSelector in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the controllerOverride setting");
        barPatronSelector = controllerOverride.barPatronSelector;

        if (dayLoader != null)
            Debug.LogError("dayLoader in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the controllerOverride setting");
        dayLoader = controllerOverride.dayLoader;

        if (validPatronNames.Count != 0)
            Debug.LogError("validPatronNames in BarSpaceControllerDebugger should not be set" +
                " as it is overriden with the controllerOverride setting");
        validPatronNames = controllerOverride.validPatronNames;
    }
}
