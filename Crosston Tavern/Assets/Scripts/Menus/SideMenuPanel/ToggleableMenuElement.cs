using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableMenuElement : MenuElement
{
    public GameObject togglablePanel;
    public GameObject openButton;
    public GameObject closeButton;

    public override void Open()
    {
        togglablePanel.SetActive(true);
        openButton.SetActive(false);
        closeButton.SetActive(true);
    }

    public override void Close()
    {
        togglablePanel.SetActive(false);
        openButton.SetActive(true);
        closeButton.SetActive(false);
    }
}
