using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectTable 
{
    [SerializeField]
    private List<EntityStatusEffect> activeEffects;

    [SerializeField]
    private List<EntityStatusEffect> timedOutEffects;

    [SerializeField]
    private StatusEffectSummary summary;

    public StatusEffectTable()
    {
        this.activeEffects = new List<EntityStatusEffect>();

        summary = new StatusEffectSummary();
    }

    public void Add(EntityStatusEffect status)
    {
        activeEffects.Add(status);
    }

    public IEnumerable ListAllStatuses()
    {
        return new List<EntityStatusEffect>( activeEffects );
    }

    public StatusEffectSummary CalculateStatus(string target="")
    {
        StatusEffectSummary s = new StatusEffectSummary();

        foreach(EntityStatusEffect effect in activeEffects) {

            EntityStatusEffectType type = effect.type;

            if (!s.ContainsKey(type)) s.Add(type, 0);
            s[type] += effect.strength;

            if (effect.target.Contains(target)) {
                s[type] += 1;
            }

        }

        return s;
    }

    public void Update()
    {
        timedOutEffects = new List<EntityStatusEffect>();
        foreach(EntityStatusEffect effect in activeEffects) {
            effect.Update();

            if (effect.duration <= 0) timedOutEffects.Add(effect);
        }

        foreach(EntityStatusEffect effect in timedOutEffects) {
            activeEffects.Remove(effect);
        }
    }
}


[System.Serializable]
public class EntityStatusEffect
{
    public string displayName;

    public string id;
    public EntityStatusEffectType type;
    public int duration;
    public int strength;
    public List<string> target;


    public EntityStatusEffect(string id, EntityStatusEffectType type, int duration, int strength, List<string> target)
    {
        this.id = id;
        this.type = type;
        this.duration = duration;
        this.strength = strength;
        this.target = new List<string>(target);

        displayName = ToString();
    }

    public void Update()
    {
        duration--;

        displayName = ToString();
    }

    public override string ToString()
    {
        string str = "<" + id + ":" + type + "-" + strength + "(" + duration + ")";

        if (target.Count > 0) str += "{" + string.Join(",", target) + "}>";
        else str += ">";

        return str;
    }
}

public enum EntityStatusEffectType
{
    //emotions:
    happy,
    angry,
    sad,
    //physical state:
    dirty,
    broken,
    injured,
    sick,
    //other:
    special
}