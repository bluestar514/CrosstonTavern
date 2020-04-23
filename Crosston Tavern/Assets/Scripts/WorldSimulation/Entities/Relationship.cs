using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship
{
    Dictionary<string, float[]> relationships;

    public enum RelationType
    {
        friendly=0,
        romantic=1
    }
    static int NUM_AXIS = 2; 


    public Relationship()
    {
        relationships = new Dictionary<string, float[]>();
    }

    public void Increase(string target, RelationType axis, float value)
    {
        if (!relationships.ContainsKey(target)) {
            relationships.Add(target, new float[NUM_AXIS]);

            for(int i = 0; i< NUM_AXIS; i++) {
                relationships[target][i] = 0;
            }
        }

        relationships[target][(int)axis] += value;
    }

    public float Get(string target, RelationType axis)
    {
        return relationships[target][(int)axis];
    }

    public List<string> GetKnownPeople()
    {
        return new List<string>(relationships.Keys);
    }
}
