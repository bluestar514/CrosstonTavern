using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WriteConversation : LogController
{
    FileHandler fh = new FileHandler("conversation", "Log");

    public override void AddConversationSeperator()
    {
        base.AddConversationSeperator();

        fh.WriteString("--CONVERSATION START--\n");
    }

    public override void AddDaySeperator(WorldTime date)
    {
        base.AddDaySeperator(date);

        fh.WriteString("----DAY START "+date.GetDate()+"----\n");
    }

    public override IEnumerator AddElement(DialogueUnit element)
    {
        yield return base.AddElement(element);

        SocialMove move = element.underpinningSocialMove;

        fh.WriteString("\t" + element.speakerName + " (" + element.emotion + "):\t" + move+ "\n" +
                    "\t\t(" + move.verb + ")\n" +
                    "\t\t\t+\t" + string.Join("\n\t\t\t \t", move.mentionedFacts) + "\n" +
                    "\t\t\t-\t" + string.Join("\n\t\t\t \t", move.retractedFacts) + "\n\n" +

                    "\t\t" + element.verbalization);
    }

    public override void Initialize(List<DialogueUnit> records)
    {
        base.Initialize(records);

        fh.NewFile();
    }

    protected override void ScrollToBottom()
    {
        base.ScrollToBottom();
        fh.WriteString("\n");
    }




}
