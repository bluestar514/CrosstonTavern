using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class WriteSimulationLog : MonoBehaviour
{
    Dictionary<string, FileHandler> fhs = new Dictionary<string, FileHandler>();

    public void Init(List<string> townieIds)
    {
        
        foreach (string id in townieIds) {
            fhs.Add(id, new FileHandler("sim_"+id, "Log"));
            fhs[id].NewFile();
        }
    }

    public void WriteEvent(ExecutedAction action)
    {
        

        fhs[action.Action.ActorId].WriteString(
            action.executionTime + " - " + action.ToString() + "\n" +
            "\tactor:\t" + action.Action.ActorId + "\n" +
            "\tfeature:\t" + action.Action.FeatureId + "\n" +
            "\tlocation:\t" + action.Action.LocationId + "\n" +
            "\teffects:\n" +
                "\t\t" + string.Join("\n\t\t", action.executedEffect) + "\n" +
            "\trationals:\n" +
                "\t\t" + string.Join("\n\t\t", action.Action.weightRationals) + "\n" +
            "\trejected actions:\n" +
                "\t\t" + string.Join("\n\t\t", action.rejectedChoices) + "\n" +
            "\tinvalid actions:\n" +
                "\t\t" + string.Join("\n\t\t", action.invalidChoices) + "\n\n"
            );

    }


    public void WriteCharacterState(Townie townie)
    {

        IEnumerable<string> inventoryContents = from item in townie.townieInformation.inventory.GetItemList()
                                                select item + ":\t" + townie.townieInformation.inventory.GetInventoryCount(item);

        List<string> relationships = new List<string>();
        Relationship rel = townie.townieInformation.relationships;
        foreach (string people in rel.GetKnownPeople()) {
            IEnumerable<string> relTags = from tag in rel.GetTag(people)
                                          select tag.ToString();

            Relationship reverseRel = townie.ws.GetRelationshipsFor(people);

            relationships.Add(
                people + ":(" +
                            rel.Get(people, Relationship.Axis.friendly) + "," +
                            rel.Get(people, Relationship.Axis.romantic) + ") - (" +
                            reverseRel.Get(townie.Id, Relationship.Axis.friendly) + "," +
                            reverseRel.Get(townie.Id, Relationship.Axis.romantic) + ") - {" +
                            string.Join(", ", relTags) + "}");
        }

        fhs[townie.Id].WriteString(
            "CurrentState:\n" +
            "\tGoals:\n" +
                "\t\t" + string.Join("\n\t\t", townie.townieInformation.knownGoals) + "\n" +
            "\tInventory:\n" +
                "\t\t" + string.Join("\n\t\t", inventoryContents) + "\n" +
            "\tRelationships:\n" +
                "\t\t" + string.Join("\n\t\t", relationships) + "\n\n");


    }
}
