using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition 
{
    public virtual bool InEffect()
    {
        return true;
    }
}
