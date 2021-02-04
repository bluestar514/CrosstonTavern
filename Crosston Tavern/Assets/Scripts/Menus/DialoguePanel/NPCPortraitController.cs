using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCPortraitController : MonoBehaviour
{
    public List<NPCPortrait> portraits;

    public List<NPCIdToPortrait> portraitNames;

    Dictionary<string, Dictionary<NPCPortrait.State, Image>> portraitDict;

    private void Awake()
    {
        portraitDict = new Dictionary<string, Dictionary<NPCPortrait.State, Image>>();

        Dictionary<NPCPortrait.NPC, string> enumToString = new Dictionary<NPCPortrait.NPC, string>();
        foreach(NPCIdToPortrait pair in portraitNames) {
            portraitDict.Add(pair.id, new Dictionary<NPCPortrait.State, Image>());
            enumToString.Add(pair.npc, pair.id);
        }

        foreach(NPCPortrait npc in portraits) {
            string name = enumToString[npc.npc];
            portraitDict[name].Add(npc.state, npc.image);
        }
    }

    public void SetPortrait(string speaker, NPCPortrait.State emotion)
    {
        foreach(NPCPortrait portrait in portraits) {
            portrait.image.gameObject.SetActive(false);
        }

        portraitDict[speaker][emotion].gameObject.SetActive(true);
    }

}

[System.Serializable]
public class NPCIdToPortrait
{
    public string id;
    public NPCPortrait.NPC npc;
}


[System.Serializable]
public class NPCPortrait
{
    public enum State
    {
        neutral,
        happy,
        angry,
        sad,
        soup
    }

    public enum NPC
    {
        bird,
        sheep,
        stickGuy
    }

    public NPC npc;
    public State state;
    public Image image;

}