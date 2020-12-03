﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogController: MonoBehaviour
{
    public GameObject contentPanel;
    public GameObject p_DisplayPanel;

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
    }
}
