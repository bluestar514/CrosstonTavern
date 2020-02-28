﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalManager 
{
    protected Map map;
    protected Registry registry;
    protected Person actor;

    public GoalManager(Map map, Registry registry, Person actor)
    {
        this.map = map;
        this.registry = registry;
        this.actor = actor;
    }

    public List<MicroEffect> GetCompletedGoals(List<MicroEffect> goals)
    {
        return new List<MicroEffect>(from goal in goals
                                     where goal.GoalComplete(map, registry, actor)
                                     select goal);
    }

    public void ClearCompletedGoals()
    {
        List<MicroEffect> allGoals = new List<MicroEffect>(actor.goalPriorityDict.Keys);
        List<MicroEffect> completedGoals = GetCompletedGoals(allGoals);
        foreach(MicroEffect goal in completedGoals) {
            actor.goalPriorityDict.Remove(goal);
        }

    }

    public void AddGoals(List<MicroEffect> goals)
    {
        Dictionary<MicroEffect, float> goalPriDict = actor.goalPriorityDict;

        List<string> keys = new List<string>(from goalPri in goalPriDict
                                             select goalPri.Key.ToString());

        foreach (MicroEffect goal in goals) {
            if (keys.Contains(goal.ToString())) {
                MicroEffect e = new List<MicroEffect>(goalPriDict.Keys).Find(g => g.ToString() == goal.ToString());

                goalPriDict[e]++;

            } else goalPriDict.Add(goal, 1);
        }
    }


    public enum State
    {
        shopKeeper,
        gatherer,
        none
    } 
}

public class FisherGoalManager: GoalManager
{
    public FisherGoalManager(Map map, Registry registry, Person actor) : base(map, registry, actor)
    {
    }

    public List<MicroEffect> GenerateNewGoals(State state)
    {
        List<MicroEffect> newGoals = new List<MicroEffect>();

        if (GetPlaceOfWork() == null) return newGoals;

        switch (state) {
            case State.shopKeeper:
                
                string location = GetPlaceOfWork().location;

                newGoals.Add(new Move(location));

                break;
            case State.gatherer:
                List<string> stock = GetPlaceOfWork().relevantResources["stock"];

                foreach(string s in stock) {
                    newGoals.Add(new InvChange(3, 1000, actor.Id, new List<string>() { s }));
                }

                newGoals.Add(new InvChange(10, 1000, actor.Id, stock));

                break;
        }

        return newGoals;
    }

    Feature GetPlaceOfWork()
    {
        string placeOfWork = actor.placeOfWork;
        if (placeOfWork == "unemployed") return null;
        return map.GetFeature(placeOfWork);
    }

    
}