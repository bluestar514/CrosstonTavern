using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WorldFactDisplayObj : MonoBehaviour
{
    public Text text;
    public WorldFact fact;

    public void Initiate(WorldFact fact)
    {
        this.fact = fact;
        this.text.text = fact.Verbalize("barkeep", "none"); 
    }
}
