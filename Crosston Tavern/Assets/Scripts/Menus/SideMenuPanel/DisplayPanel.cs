using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPanel<T> : MonoBehaviour
{
    public Text factText;

    T fact;

    public virtual void Fill(T fact)
    {
        this.fact = fact;

        factText.text = fact.ToString();
    }
}
