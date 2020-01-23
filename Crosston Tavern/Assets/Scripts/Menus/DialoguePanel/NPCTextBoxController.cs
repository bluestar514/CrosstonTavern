using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTextBoxController : MenuElement
{
    public Text speachText;

    public void Fill(string dialogue)
    {
        speachText.text = dialogue;
    }
}