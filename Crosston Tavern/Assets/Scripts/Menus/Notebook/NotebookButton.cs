using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookButton : MonoBehaviour
{
    public Text text;
    string key;
    MainNotebookTab mainPanel;

    public void Init(string text, MainNotebookTab panel)
    {
        key = text;
        this.text.text = VerbalizationDictionary.CapFirstLetter( VerbalizationDictionary.Replace( text ));
        mainPanel = panel;
    }

    public void OnClick()
    {
        mainPanel.OpenSubTab(key);
    }
}
