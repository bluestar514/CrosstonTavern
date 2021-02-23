using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMoveElement 
{
    public string id;

    public Func<SocialMove> socialMove;

    public Func<List<DialogueMoveElement>> next;

    public DialogueMoveElement(string id, Func<SocialMove> socialMove, Func<List<DialogueMoveElement>> next)
    {
        this.id = id;
        this.socialMove = socialMove;
        this.next = next;
    }
}
