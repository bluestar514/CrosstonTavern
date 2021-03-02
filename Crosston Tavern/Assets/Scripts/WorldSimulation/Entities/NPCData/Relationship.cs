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

    public enum RelationType
    {
        friendly=0,
        romantic=1
    }
    static int NUM_AXIS = 2;

    public static List<RelationshipTag> romanticTags = new List<RelationshipTag>() {
        RelationshipTag.dating,
        RelationshipTag.crushing_on,
        RelationshipTag.in_love_with,
        RelationshipTag.head_over_heels
    };

    public static List<RelationshipTag>friendlyTags = new List<RelationshipTag>() {
        RelationshipTag.rivals,
        RelationshipTag.liked,
        RelationshipTag.friend,
        RelationshipTag.bestFriend,
        RelationshipTag.disliked,
        RelationshipTag.enemy,
        RelationshipTag.nemisis
    };


    public enum RelationshipTag
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

    static public Dictionary<RelationshipTag, Dictionary<RelationType, float[]>> codifiedRelationRanges =
        new Dictionary<RelationshipTag, Dictionary<RelationType, float[]>>() {
            {RelationshipTag.acquantences, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-maxValue, maxValue} },
                {RelationType.romantic, new float[]{ -maxValue, maxValue } }
            } },
            { RelationshipTag.liked, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{low, medium} },
                {RelationType.romantic, new float[]{-maxValue, maxValue } }
            } },
            { RelationshipTag.friend, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{medium, high} },
                {RelationType.romantic, new float[]{-maxValue, maxValue } }
            } },
            { RelationshipTag.bestFriend, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{70, maxValue } },
                {RelationType.romantic, new float[]{-maxValue, maxValue } }
            } },

            { RelationshipTag.crushing_on, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-maxValue, maxValue } },
                {RelationType.romantic, new float[]{ low, medium } }
            } },
            { RelationshipTag.in_love_with, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{0, maxValue } },
                {RelationType.romantic, new float[]{ medium, high } }
            } },
            { RelationshipTag.head_over_heels, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{0, maxValue } },
                {RelationType.romantic, new float[]{ high, maxValue } }
            } },
            
            { RelationshipTag.disliked, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-medium, -low} },
                {RelationType.romantic, new float[]{-maxValue, maxValue } }
            } },
            { RelationshipTag.enemy, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-high, -medium} },
                {RelationType.romantic, new float[]{-maxValue, maxValue } }
            } },
            { RelationshipTag.nemisis, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-maxValue, -high } },
                {RelationType.romantic, new float[]{-maxValue, maxValue } }
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
    public void Set(string target, RelationType axis, float value)
    {
        if (!relationships.ContainsKey(target)) {
            relationships.Add(target, new float[NUM_AXIS]);

            for (int i = 0; i < NUM_AXIS; i++) {
                relationships[target][i] = 0;
            }
        }

        relationships[target][(int)axis] = value;
    }

    /// <summary>
    /// Main method of altering the relationship state between characters. Use this, not Set during simulation
    /// </summary>
    /// <param name="target"></param>
    /// <param name="axis"></param>
    /// <param name="value"></param>
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

    /// <summary>
    /// Main method for getting the value of a given relation
    /// </summary>
    /// <param name="target"></param>
    /// <param name="axis"></param>
    /// <returns>Returns an int for some reason, despite this value being 
    /// stored internally as a float... Not sure whats up with that.</returns>
    public int Get(string target, RelationType axis)
    {
        if (relationships.ContainsKey(target))
            return (int)relationships[target][(int)axis];
        else return 0;
    }

    public void AddRelationTag(string target, RelationshipTag tag)
    {
        if (!relationTags.ContainsKey(target)) relationTags.Add(target, new List<RelationshipTag>());

        if(!RelationTagged(target, tag)) relationTags[target].Add(tag);

        
        if (codifiedRelationRanges.ContainsKey(tag)) {
            if (!relationships.ContainsKey(target)) {
                Set(target, RelationType.friendly, 0);
                Set(target, RelationType.romantic, 0);
            }
            foreach (RelationType axis in new List<RelationType>() { RelationType.romantic, RelationType.friendly }) {
                relationships[target][(int)axis] = Mathf.Clamp(relationships[target][(int)axis],
                                                                codifiedRelationRanges[tag][axis][0],
                                                                codifiedRelationRanges[tag][axis][1]);
            }
        }
    }

    public void RemoveRelationTag(string target, RelationshipTag tag)
    {
        if (relationTags.ContainsKey(target)) {
            relationTags[target].Remove(tag);
        }
    }
    public List<RelationshipTag> GetTag(string target)
    {
        if (!relationTags.ContainsKey(target)) return new List<RelationshipTag>();

        else return new List<RelationshipTag>(relationTags[target]);
    }

    public bool RelationTagged(string target, RelationshipTag tag)
    {
        if (!relationTags.ContainsKey(target)) return false;

        return relationTags[target].Contains(tag);
    }

    public List<string> GetKnownPeople()
    {
        return new List<string>(relationships.Keys);
    }

}
