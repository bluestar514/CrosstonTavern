using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WriteConversation : LogController
{

    public string pathHeader = "Assets/Resources/";
    public string path = "test.txt";

    string runPath;

    public override void AddConversationSeperator()
    {
        base.AddConversationSeperator();

        WriteString("--CONVERSATION START--\n");
    }

    public override void AddDaySeperator(WorldTime date)
    {
        base.AddDaySeperator(date);

        WriteString("----DAY START "+date.GetDate()+"----\n");
    }

    public override void AddElement(DialogueUnit element)
    {
        base.AddElement(element);

        SocialMove move = element.underpinningSocialMove;

        WriteString("\t" + element.speakerName + " (" + element.emotion + "):\t" + move+ "\n" +
                    "\t\t(" + move.verb + ")\n" +
                    "\t\t\t+\t" + string.Join("\n\t\t\t \t", move.mentionedFacts) + "\n" +
                    "\t\t\t-\t" + string.Join("\n\t\t\t \t", move.retractedFacts) + "\n\n" +

                    "\t\t" + element.verbalization);
    }

    public override void Initialize(List<DialogueUnit> records)
    {
        base.Initialize(records);


        runPath = GetPath();


        StreamWriter writer = new StreamWriter(runPath, false);

        System.DateTime time = System.DateTime.Now;

        writer.WriteLine(time.ToString());
        writer.Close();
    }

    protected override void ScrollToBottom()
    {
        base.ScrollToBottom();
        WriteString("\n");
    }

    void WriteString( string printedText )
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(runPath, true);
        writer.WriteLine(printedText);
        writer.Close();
    }


    string GetPath()
    {
        string path = "";

#if UNITY_EDITOR
            path = pathHeader + this.path;
#else
            path = Application.dataPath + "/"+ this.path;
            System.DateTime time = System.DateTime.Now;
            path = path.Replace(".txt", time.ToString("yyyy-MM-dd-_HH-mm-ss") + ".txt");
            
#endif
        
        return path;
    }
}
