using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerTagController : MenuElement
{
    public Text speakerNameTagText;

    public void Fill(string dialogue)
    {
        speakerNameTagText.text = dialogue;
    }
}
