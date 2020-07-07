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

        summary = CalculateStatus();
    }

    public void Reduce(EntityStatusEffectType type, int amount)
    {
        foreach(EntityStatusEffect effect in activeEffects) {
            if (effect.type == type) effect.duration -= amount;
        }

        summary = CalculateStatus();
    }

    public IEnumerable ListAllStatuses()
    {
        return new List<EntityStatusEffect>( activeEffects );
    }

    public StatusEffectSummary CalculateStatus(string target="")
    {
        StatusEffectSummary s = new StatusEffectSummary();

        foreach(EntityStatusEffectType status in System.Enum.GetValues(typeof(EntityStatusEffectType))) {
            s.Add(status, 0);
        }

        foreach(EntityStatusEffect effect in activeEffects) {

            EntityStatusEffectType type = effect.type;

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


        summary = CalculateStatus();
    }

    public StatusEffectTable Copy()
    {
        StatusEffectTable table = new StatusEffectTable();
        foreach(EntityStatusEffect effect in activeEffects) {
            table.Add(new EntityStatusEffect( effect));
        }

        table.summary = table.CalculateStatus();

        return table;

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

    public EntityStatusEffect(EntityStatusEffect effect) : this(effect.id, effect.type, effect.duration, effect.strength, effect.target) { }

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