using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    public Text text;


    public void Set(string text)
    {
        this.text.text = text;
    }
}
