using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WriteConversation : LogController
{

    public string pathHeader = "Assets/Resources/";
    public string path = "test.txt";

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

        WriteString("\t" + element.speakerName + " (" + element.emotion + "):\t" +
                    element.underpinningSocialMove + "\n\n" +
                    "\t\t" + element.verbalization);
    }

    public override void Initialize(List<DialogueUnit> records)
    {
        string path = pathHeader + this.path;

        StreamWriter writer = new StreamWriter(path, false);

        System.DateTime time = System.DateTime.Now;

        writer.WriteLine(time.ToString());
        writer.Close();
    }

    protected override void GenerateFactPanels(List<DialogueUnit> knownRecords)
    {
        base.GenerateFactPanels(knownRecords);
    }

    protected override void ScrollToBottom()
    {
        base.ScrollToBottom();
        WriteString("\n");
    }

    void WriteString( string printedText )
    {
        string path = pathHeader + this.path;

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(printedText);
        writer.Close();
    }
}
