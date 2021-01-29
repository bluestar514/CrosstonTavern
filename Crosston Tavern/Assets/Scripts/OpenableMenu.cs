using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableMenu : MonoBehaviour
{
    public GameObject togglablePanel; 

    public void Close()
    {
        togglablePanel.SetActive(false);
    }
    public void Open()
    {
        togglablePanel.SetActive(true);
    }
}
