using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relationship
{
    [SerializeField]
    StringFloatArrayDictionary relationships;

    [SerializeField]
    StringRelationTagsDictionary relationTags;

    public enum Axis
    {
        friendly=0,
        romantic=1
    }
    static int NUM_AXIS = 2;

    public static List<Tag> romanticTags = new List<Tag>() {
        Tag.dating,
        Tag.crushing_on,
        Tag.in_love_with,
        Tag.head_over_heels
    };

    public static List<Tag>friendlyTags = new List<Tag>() {
        Tag.rivals,
        Tag.liked,
        Tag.friend,
        Tag.bestFriend,
        Tag.disliked,
        Tag.enemy,
        Tag.nemisis
    };


    public enum Tag
    {
        self,

        //social choices:
        dating,
        rivals,

        //passiveStates:
        acquantences,

        liked,
        friend,
        bestFriend,

        no_affection,
        crushing_on,
        in_love_with,
        head_over_heels,

        disliked,
        enemy,
        nemisis,

        //unused:
        family
    }

    static int low = 15;
    static int medium = 35;
    static int high = 70;
    public static int maxValue = 100;

    static public Dictionary<Tag, Dictionary<Axis, float[]>> codifiedRelationRanges =
        new Dictionary<Tag, Dictionary<Axis, float[]>>() {
            {Tag.acquantences, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{-maxValue, maxValue} },
                {Axis.romantic, new float[]{ -maxValue, maxValue } }
            } },
            { Tag.liked, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{low, medium} },
                {Axis.romantic, new float[]{-maxValue, maxValue } }
            } },
            { Tag.friend, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{medium, high} },
                {Axis.romantic, new float[]{-maxValue, maxValue } }
            } },
            { Tag.bestFriend, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{70, maxValue } },
                {Axis.romantic, new float[]{-maxValue, maxValue } }
            } },

            { Tag.no_affection, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{-maxValue, maxValue } },
                {Axis.romantic, new float[]{ -maxValue, 0 } }
            } },
            { Tag.crushing_on, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{-maxValue, maxValue } },
                {Axis.romantic, new float[]{ low, medium } }
            } },
            { Tag.in_love_with, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{0, maxValue } },
                {Axis.romantic, new float[]{ medium, high } }
            } },
            { Tag.head_over_heels, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{0, maxValue } },
                {Axis.romantic, new float[]{ high, maxValue } }
            } },
            
            { Tag.disliked, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{-medium, -low} },
                {Axis.romantic, new float[]{-maxValue, maxValue } }
            } },
            { Tag.enemy, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{-high, -medium} },
                {Axis.romantic, new float[]{-maxValue, maxValue } }
            } },
            { Tag.nemisis, new Dictionary<Axis, float[]> {
                {Axis.friendly, new float[]{-maxValue, -high } },
                {Axis.romantic, new float[]{-maxValue, maxValue } }
            } }
        };


    public Relationship()
    {
        relationships = new StringFloatArrayDictionary();
        relationTags = new StringRelationTagsDictionary();
    }

    protected Relationship(StringFloatArrayDictionary relationships, StringRelationTagsDictionary relationTags)
    {
        this.relationships = new StringFloatArrayDictionary();
        this.relationships.CopyFrom(relationships);

        this.relationTags = new StringRelationTagsDictionary();
        this.relationTags.CopyFrom(relationTags);

    }

    public Relationship Copy(bool perfect)
    {
        if (perfect) return new Relationship(relationships, relationTags);
        else return new Relationship();
    }

    /// <summary>
    /// Used for initializing characters! Not updating them during the simulation!
    /// </summary>
    /// <param name="target"></param>
    /// <param name="axis"></param>
    /// <param name="value"></param>
    public void Set(string target, Axis axis, float value)
    {
        if (!relationships.ContainsKey(target)) {
            relationships.Add(target, new float[NUM_AXIS]);

            for (int i = 0; i < NUM_AXIS; i++) {
                relationships[target][i] = 0;
            }
        }

        relationships[target][(int)axis] = value;

        UpdateTags(target);

    }

    /// <summary>
    /// Main method of altering the relationship state between characters. Use this, not Set during simulation
    /// </summary>
    /// <param name="target"></param>
    /// <param name="axis"></param>
    /// <param name="value"></param>
    public void Increase(string target, Axis axis, float value)
    {
        if (!relationships.ContainsKey(target)) {
            relationships.Add(target, new float[NUM_AXIS]);

            for(int i = 0; i< NUM_AXIS; i++) {
                relationships[target][i] = 0;
            }
        }

        relationships[target][(int)axis] += value;
        relationships[target][(int)axis] = Mathf.Clamp(relationships[target][(int)axis], -maxValue, maxValue);

        UpdateTags(target);
    }

    /// <summary>
    /// Main method for getting the value of a given relation
    /// </summary>
    /// <param name="target"></param>
    /// <param name="axis"></param>
    /// <returns>Returns an int for some reason, despite this value being 
    /// stored internally as a float... Not sure whats up with that.</returns>
    public int Get(string target, Axis axis)
    {
        if (relationships.ContainsKey(target))
            return (int)relationships[target][(int)axis];
        else return 0;
    }

    public void AddRelationTag(string target, Tag tag)
    {
        if (!relationTags.ContainsKey(target)) relationTags.Add(target, new List<Tag>());

        if(!RelationTagged(target, tag)) relationTags[target].Add(tag);

        
        if (codifiedRelationRanges.ContainsKey(tag)) {
            if (!relationships.ContainsKey(target)) {
                Set(target, Axis.friendly, 0);
                Set(target, Axis.romantic, 0);
            }
            foreach (Axis axis in new List<Axis>() { Axis.romantic, Axis.friendly }) {
                relationships[target][(int)axis] = Mathf.Clamp(relationships[target][(int)axis],
                                                                codifiedRelationRanges[tag][axis][0],
                                                                codifiedRelationRanges[tag][axis][1]);
            }
        }

        UpdateTags(target);
    }

    public void RemoveRelationTag(string target, Tag tag)
    {
        if (relationTags.ContainsKey(target)) {
            relationTags[target].Remove(tag);
        }
    }
    public List<Tag> GetTag(string target)
    {
        if (!relationTags.ContainsKey(target)) return new List<Tag>();

        else return new List<Tag>(relationTags[target]);
    }

    public bool RelationTagged(string target, Tag tag)
    {

        if (!relationTags.ContainsKey(target)) return false;

        return relationTags[target].Contains(tag);
    }

    public List<string> GetKnownPeople()
    {
        return new List<string>(relationships.Keys);
    }



    void UpdateTags(string target)
    {
        if (!relationships.ContainsKey(target)) return;
        
        
        List<Tag> removedTags = new List<Tag>();
        if (relationTags.ContainsKey(target)) {    
            foreach (Tag tag in relationTags[target]) {
                if (!codifiedRelationRanges.ContainsKey(tag)) continue;
                foreach (Axis axis in new List<Axis>() { Axis.romantic, Axis.friendly }) {
                    if (relationships[target][(int)axis] < codifiedRelationRanges[tag][axis][0] ||
                        relationships[target][(int)axis] > codifiedRelationRanges[tag][axis][1]) {
                        removedTags.Add(tag);
                    }
                }
            }

            foreach (Tag tag in removedTags) {
                relationTags[target].Remove(tag);
            }
        } else {
            relationTags.Add(target, new List<Tag>());
        }

        foreach (Tag tag in codifiedRelationRanges.Keys) {
            if (relationTags[target].Contains(tag)) continue;
            if (removedTags.Contains(tag)) continue;

            bool valid = true;
            foreach (Axis axis in new List<Axis>() { Axis.romantic, Axis.friendly }) {
                if (relationships[target][(int)axis] < codifiedRelationRanges[tag][axis][0] ||
                    relationships[target][(int)axis] > codifiedRelationRanges[tag][axis][1]) {
                    valid = false;
                }
                
            }
            if(valid) relationTags[target].Add(tag);
        }
    }
}
