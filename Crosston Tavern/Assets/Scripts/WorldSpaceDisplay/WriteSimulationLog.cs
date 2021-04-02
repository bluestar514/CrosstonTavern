using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class WriteSimulationLog : MonoBehaviour
{
    public string pathHeader = "Assets/Resources";
    public string path = "eventLog";

    public void Init(List<string> townieIds)
    {
        pathHeader = GetPathHeader();

        System.DateTime time = System.DateTime.Now;
        string date = time.ToString("yyyy-MM-dd_HH-mm-ss");
        pathHeader +="/"+ path+"_"+date + "/";

        Directory.CreateDirectory(pathHeader);
        

        foreach(string id in townieIds) {
            StreamWriter writer = new StreamWriter(GetCharacterFile(id), false);

            writer.WriteLine(time.ToString());
            writer.Close();
        }
    }

    string GetPathHeader()
    {
        string path = "";

#if UNITY_EDITOR
        path = pathHeader;
#else
        path = Application.dataPath;
#endif

        return path;
    }

    string GetCharacterFile(string townieId)
    {
         return pathHeader + townieId + ".txt"; 
    }

    public void WriteEvent(ExecutedAction action)
    {
        string path = GetCharacterFile(action.Action.ActorId);

        StreamWriter writer = new StreamWriter(path, true);

        writer.WriteLine(
            action.executionTime + " - " + action.ToString() + "\n"+
            "\tactor:\t"+ action.Action.ActorId+ "\n"+
            "\tfeature:\t" + action.Action.FeatureId + "\n" +
            "\tlocation:\t" + action.Action.LocationId + "\n" +
            "\teffects:\n"+
                "\t\t"+ string.Join("\n\t\t", action.executedEffect) +"\n"+
            "\trationals:\n" + 
                "\t\t" + string.Join("\n\t\t", action.Action.weightRationals) + "\n"+
            "\trejected actions:\n" +
                "\t\t" + string.Join("\n\t\t", action.rejectedChoices) + "\n" +
            "\tinvalid actions:\n" +
                "\t\t" + string.Join("\n\t\t", action.invalidChoices) + "\n\n" 
            );
        
        writer.Close();
    }


    public void WriteCharacterState(Townie townie)
    {
        string path = GetCharacterFile(townie.Id);

        StreamWriter writer = new StreamWriter(path, true);

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

        writer.WriteLine(
            "CurrentState:\n" +
            "\tGoals:\n" +
                "\t\t" + string.Join("\n\t\t", townie.townieInformation.knownGoals) + "\n" +
            "\tInventory:\n" +
                "\t\t" + string.Join("\n\t\t", inventoryContents) + "\n" +
            "\tRelationships:\n" +
                "\t\t" + string.Join("\n\t\t", relationships) + "\n\n");



        writer.Close();
    }
}
