using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//For the moment they are just fancy strings, 
//but eventually we can imagine they have more 
//complicated structure holding far more information
//maybe even information on how to be displayed

[System.Serializable]
public class SocialMove
{
    public string verb;

    public override string ToString()
    {
        return "<"+verb+">";
    }
}
