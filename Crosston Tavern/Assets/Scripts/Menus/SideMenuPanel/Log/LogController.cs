using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogController: MonoBehaviour
{
    public GameObject contentPanel;
    public GameObject p_DisplayPanel;
    public GameObject daySeperatorPrefab;
    public GameObject conversationSeperatorPrefab;

    public DialogueBoxController dialogueBoxController;

    public ScrollRect scrollRect;

    List<RecordDisplay> displayPanels = new List<RecordDisplay>();

    Coroutine scrollFunction;

    bool scrollDown = false;
    public float dialogueDefaultTextSpeed;

    public virtual void Initialize(BarSpaceController bsc)
    {
        dialogueBoxController.Initialize(bsc);
    }

    private void Update()
    {
        if (scrollDown) {
            scrollRect.verticalNormalizedPosition = 0;
        }
    }

    public virtual IEnumerator AddElement(DialogueUnit element)
    {
        Debug.Log("LogController: Adding new dialogue unit element: " + element);

        RecordDisplay panel = Instantiate(p_DisplayPanel, contentPanel.transform).GetComponent<RecordDisplay>();

        scrollDown = true;

        panel.Init(dialogueDefaultTextSpeed);
        yield return panel.Fill(element);

        displayPanels.Add(panel);

        scrollDown = false;

    }

    public virtual void AddDaySeperator(WorldTime date)
    {
        Instantiate(daySeperatorPrefab, contentPanel.transform).GetComponent<DaySeperator>().Initialize(date);
        ScrollToBottom();
    }

    public virtual void AddConversationSeperator()
    {
        Instantiate(conversationSeperatorPrefab, contentPanel.transform);
        ScrollToBottom();
    }

    protected virtual void ScrollToBottom()
    {

        scrollFunction = StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        Debug.Log("LogController: Starting scroll down");


        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;

    }



    public void DisplayPlayerActions(List<DialogueUnit> dialogueUnits)
    {
        dialogueBoxController.DisplayPlayerActions(dialogueUnits);
        ScrollToBottom();
    }

    public void DisplayNPCAction(DialogueUnit dialogueUnit)
    {
        dialogueBoxController.DisplayNPCAction(dialogueUnit);
        ScrollToBottom();
    }
}
