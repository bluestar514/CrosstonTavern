using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaySeperator : MonoBehaviour
{
    public Text text;

    public void Initialize(WorldTime date)
    {
        text.text = date.GetDate();
    }
}
