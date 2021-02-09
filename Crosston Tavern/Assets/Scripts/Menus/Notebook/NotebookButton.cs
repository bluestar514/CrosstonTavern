using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookButton : MonoBehaviour
{
    public Text text;
    MainNotebookTab mainPanel;

    public void Init(string text, MainNotebookTab panel)
    {
        this.text.text = VerbalizationDictionary.Replace( text );
        mainPanel = panel;
    }

    public void OnClick()
    {
        mainPanel.OpenSubTab(text.text);
    }
}
