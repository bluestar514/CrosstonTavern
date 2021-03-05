using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RelationsPanel : MonoBehaviour
{
    string source;
    string target;

    public Text tagsSourceToTarget;
    public Text tagsTargetToSource;

    public Image arrowSourceToTarget;
    public Image arrowTargetToSource;

    public NPCPortraitController sourceImage;
    public NPCPortraitController targetImage;

    public SetText sourceName;
    public SetText targetName;

    public void Init(string source, string target)
    {
        this.source = source;
        this.target = target;

        sourceImage.Awake();
        targetImage.Awake();
    }


    public void UpdateStats(List<StateRelation> relationships)
    {
        sourceImage.SetPortrait(source, NPCPortrait.State.neutral);
        targetImage.SetPortrait(target, NPCPortrait.State.neutral);
        sourceName.Set(VerbalizationDictionary.Replace(source));
        targetName.Set(VerbalizationDictionary.Replace(target));

        arrowSourceToTarget.color = Color.gray;
        arrowTargetToSource.color = Color.gray;


        List<Relationship.Tag> tagsSourceToTarget = new List<Relationship.Tag>();
        List<Relationship.Tag> tagsTargetToSource = new List<Relationship.Tag>();

        foreach (StateRelation rel in relationships) {
            if(rel.source == source) {
                tagsSourceToTarget.Add(rel.tag);
            } else {
                tagsTargetToSource.Add(rel.tag);
            }
        }

        RemoveRedundantTags(tagsTargetToSource);
        RemoveRedundantTags(tagsSourceToTarget);

        this.tagsSourceToTarget.text = string.Join("\n", new List<string>(from tag in tagsSourceToTarget
                                                                          select TagToString(tag)));
        this.tagsTargetToSource.text = string.Join("\n", new List<string>(from tag in tagsTargetToSource
                                                                          select TagToString(tag)));
        if (tagsSourceToTarget.Count == 0) this.tagsSourceToTarget.text = "Unknown";
        if (tagsTargetToSource.Count == 0) this.tagsTargetToSource.text = "Unknown";
    }

    void RemoveRedundantTags(List<Relationship.Tag> tags)
    {
        if (tags.Contains(Relationship.Tag.head_over_heels)) {
            tags.Remove(Relationship.Tag.in_love_with);
            tags.Remove(Relationship.Tag.crushing_on);
        }else if (tags.Contains(Relationship.Tag.in_love_with)) {
            tags.Remove(Relationship.Tag.crushing_on);
        }

        if (tags.Contains(Relationship.Tag.bestFriend)) {
            tags.Remove(Relationship.Tag.friend);
            tags.Remove(Relationship.Tag.liked);
        } else if (tags.Contains(Relationship.Tag.friend)) {
            tags.Remove(Relationship.Tag.liked);
        }

        if (tags.Contains(Relationship.Tag.nemisis)) {
            tags.Remove(Relationship.Tag.enemy);
            tags.Remove(Relationship.Tag.disliked);
        } else if (tags.Contains(Relationship.Tag.enemy)) {
            tags.Remove(Relationship.Tag.disliked);
        }

        if (tags.Contains(Relationship.Tag.no_affection)) tags.Remove(Relationship.Tag.no_affection);

        if (tags.Count > 1)
            tags.Remove(Relationship.Tag.acquantences);
    }

    string TagToString(Relationship.Tag tag)
    {
        switch (tag) {
            case Relationship.Tag.head_over_heels:
                return "Deeply In Love";
            case Relationship.Tag.in_love_with:
                return "In Love";
            case Relationship.Tag.crushing_on:
                return "Crush";
            case Relationship.Tag.bestFriend:
                return "Best Friends";
            case Relationship.Tag.friend:
                return "Friends";
            case Relationship.Tag.liked:
                return "Positive Opinion";
            case Relationship.Tag.nemisis:
                return "Hated";
            case Relationship.Tag.enemy:
                return "Disliked";
            case Relationship.Tag.disliked:
                return "Negative Opinion";
            case Relationship.Tag.dating:
                return "Dating";
            case Relationship.Tag.acquantences:
                return "Acquantences";
            default:
                return tag.ToString();

        }
    }
}
