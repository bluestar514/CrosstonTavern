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

    public ScrollRect scrollRect;

    List<RecordDisplay> displayPanels = new List<RecordDisplay>();

    Coroutine scrollFunction;

    public virtual void Initialize()
    {
        
    }

    public virtual IEnumerator AddElement(DialogueUnit element)
    {
        Debug.Log("LogController: Adding new dialogue unit element: " + element);

        RecordDisplay panel = Instantiate(p_DisplayPanel, contentPanel.transform).GetComponent<RecordDisplay>();

        ScrollToBottom();

        yield return panel.Fill(element);

        displayPanels.Add(panel);

        StopCoroutine(scrollFunction);
        Debug.Log("LogController: stopping auto scroll");

    }

    public virtual void AddDaySeperator(WorldTime date)
    {
        Instantiate(daySeperatorPrefab, contentPanel.transform).GetComponent<DaySeperator>().Initialize(date);
        //ScrollToBottom();
    }

    public virtual void AddConversationSeperator()
    {
        Instantiate(conversationSeperatorPrefab, contentPanel.transform);
        //ScrollToBottom();
    }

    protected virtual void ScrollToBottom()
    {

        scrollFunction = StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        Debug.Log("LogController: Starting scroll down");

        while (true) {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }
    }
}
