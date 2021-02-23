using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEntry : MonoBehaviour
{
    public Text text;

    public void Init(string text)
    {
        this.text.text = text;
    }
}
