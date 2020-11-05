using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueUnit
{
    public string verbalization;
    public string speakerName;
    public SocialMove underpinningSocialMove;
    public List<Fact> facts;
    // also probably would hold:
    //  - speaker's emotion being displayed
    
    List<Fact> potentialFacts = new List<Fact>()
    {
        new Fact("Alicia",  "is", "busy"),
        new Fact("The clinic", "is", "messy"),
        new Fact("Rayne", "is", "unemployed"),
        new Fact("Icecream", "is", "tasty")
    };

    public DialogueUnit(string verbalization, string speakerName, SocialMove underpinningSocialMove)
    {
        this.verbalization = underpinningSocialMove.ToString();
        this.speakerName = speakerName;
        this.underpinningSocialMove = underpinningSocialMove;
        this.facts = pickRandomFacts(Mathf.FloorToInt(Random.value * potentialFacts.Count));
    }

    List<Fact> pickRandomFacts(int number)
    {
        List<Fact> randomFacts = new List<Fact>();

        if (number >= potentialFacts.Count) return potentialFacts;

        while (randomFacts.Count < number)
        {
            Fact randomFact = potentialFacts[Mathf.FloorToInt(Random.value * potentialFacts.Count)];
            if (!randomFacts.Contains(randomFact))
            {
                randomFacts.Add(randomFact);
            }
        }

        return randomFacts;
    }

    public override string ToString()
    {
        return "{"+speakerName+"("+underpinningSocialMove+"): \""+verbalization+"\"}";
    }
}

