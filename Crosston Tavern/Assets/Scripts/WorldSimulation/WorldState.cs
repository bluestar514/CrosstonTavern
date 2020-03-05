using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState 
{
    public Map map;
    public Registry registry;
    public WorldTime time;

    public WorldState(Map map, Registry registry, WorldTime time)
    {
        this.map = map;
        this.registry = registry;
        this.time = time;
    }


    public void Tick(int t=1)
    {
        time.Tick(t);
    }
}
