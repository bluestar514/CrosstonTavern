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

    public void Initialize(List<DialogueUnit> records)
    {
        GenerateFactPanels(records);
    }

    void GenerateFactPanels(List<DialogueUnit> knownRecords)
    {
        foreach (DialogueUnit record in knownRecords) {
            AddElement(record);
        }
    }

    public void AddElement(DialogueUnit element)
    {
        RecordDisplay panel = Instantiate(p_DisplayPanel, contentPanel.transform).GetComponent<RecordDisplay>();

        panel.Fill(element);

        displayPanels.Add(panel);

        ScrollToBottom();
    }

    public void AddDaySeperator(WorldTime date)
    {
        Instantiate(daySeperatorPrefab, contentPanel.transform).GetComponent<DaySeperator>().Initialize(date);
        ScrollToBottom();
    }

    public void AddConversationSeperator()
    {
        Instantiate(conversationSeperatorPrefab, contentPanel.transform);
        ScrollToBottom();
    }


    private void ScrollToBottom()
    {

        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }
}
