using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPanel<T> : MonoBehaviour
{
    public Text factText;

    protected T fact;

    public virtual IEnumerator Fill(T fact)
    {
        this.fact = fact;

        factText.text = fact.ToString();

        yield break;
    }

    public bool Matches(T fact)
    {
        return this.fact.Equals(fact);
    }
}
