using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fact
{
    public string subject;
    public string verb;
    public string obj;

    public Fact(string subject, string verb, string obj)
    {
        this.subject = subject;
        this.verb = verb;
        this.obj = obj;
    }

    public override string ToString()
    {
        return subject + " "+ verb+" "+ obj;
    }
}
