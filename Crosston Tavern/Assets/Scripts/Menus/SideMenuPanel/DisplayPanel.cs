using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPanel<T> : MonoBehaviour
{
    public Text factText;

    protected T fact;

    public virtual void Fill(T fact)
    {
        this.fact = fact;

        factText.text = fact.ToString();
    }

    public bool Matches(T fact)
    {
        return this.fact.Equals(fact);
    }
}
