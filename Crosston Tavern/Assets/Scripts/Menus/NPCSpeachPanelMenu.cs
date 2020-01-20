using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCSpeachPanelMenu : MenuElement
{
    public Text speachText;

    public void Fill(SocialMove socialMove)
    {
        speachText.text = socialMove.ToString();
    }

}
