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

    public enum CodifiedRelationships
    {
        acquantences,
        friends,
        lovers,
        crushes,
        enemies
    }

    public enum RelationshipTag
    {
        dating,
        rivals,
        family
    }

    static public Dictionary<CodifiedRelationships, Dictionary<RelationType, float[]>> codifiedRelationRanges =
        new Dictionary<CodifiedRelationships, Dictionary<RelationType, float[]>>() {
            {CodifiedRelationships.acquantences, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-2, 2} },
                {RelationType.romantic, new float[]{-4, 4} }
            } },
            { CodifiedRelationships.friends, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{2, 100} },
                {RelationType.romantic, new float[]{-100, 100} }
            } },
            { CodifiedRelationships.lovers, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{0, 100} },
                {RelationType.romantic, new float[]{4, 100} }
            } },
            { CodifiedRelationships.enemies, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-100, -2} },
                {RelationType.romantic, new float[]{-100, 100} }
            } },
            { CodifiedRelationships.crushes, new Dictionary<RelationType, float[]> {
                {RelationType.friendly, new float[]{-100, 100} },
                {RelationType.romantic, new float[]{2, 100} }
            } }
        };


    public Relationship()
    {
        relationships = new StringFloatArrayDictionary();
        relationTags = new StringRelationTagsDictionary();
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

    public int Get(string target, RelationType axis)
    {
        if (target.StartsWith("person_")) target = target.Replace("person_", "");

        if (relationships.ContainsKey(target))
            return (int)relationships[target][(int)axis];
        else return 0;
    }

    public void AddRelationTag(string target, RelationshipTag tag)
    {
        if (!relationTags.ContainsKey(target)) relationTags.Add(target, new List<RelationshipTag>());

        if(!RelationTagged(target, tag)) relationTags[target].Add(tag);
    }

    public void RemoveRelationTag(string target, RelationshipTag tag)
    {
        if (relationTags.ContainsKey(target)) {
            relationTags[target].Remove(tag);
        }
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

    public bool HasRelation(string other, CodifiedRelationships rel)
    {
        switch (rel) {
            case CodifiedRelationships.friends:
                return IsFriend(other);
            case CodifiedRelationships.enemies:
                return IsEnemy(other);
            case CodifiedRelationships.crushes:
                return HasCrush(other);
            case CodifiedRelationships.acquantences:
                return !IsEnemy(other) && !IsFriend(other);
        }

        return false;
    }

    public bool IsFriend(string other)
    {

        return relationships.ContainsKey(other) && 
            relationships[other][(int)RelationType.friendly] > 2;
    }

    public bool IsEnemy(string other)
    {
        return relationships.ContainsKey(other) && 
            relationships[other][(int)RelationType.friendly] < -2;
    }

    public bool HasCrush(string other)
    {
        return relationships.ContainsKey(other) && 
            relationships[other][(int)RelationType.romantic] > 2;
    }

    public bool IsDisgusted(string other)
    {
        return relationships.ContainsKey(other) && 
            relationships[other][(int)RelationType.romantic] < -2;
    }
}
