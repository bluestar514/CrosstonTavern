using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patron 
{
    string name;
    public string Name { get => name; set => name = value; }


    List<SocialMove> socialMoves;

    public Patron(string name)
    {
        this.Name = name;

        socialMoves = new List<SocialMove>()
        {
            new SocialMove("talk"),
            new SocialMove("argue"),
            new SocialMove("insult"),
            new SocialMove("compliment"),
            new SocialMove("complain"),
            new SocialMove("brag"),
            new SocialMove("askAbout"),
            new SocialMove("smallTalk"),
            new SocialMove("agree"),
            new SocialMove("disagree"),
            new SocialMove("gossip")
        };
    }

    public SocialMove PickSocialMove()
    {
        return PickBestSocialMoves(1)[0];
    }

    public List<SocialMove> PickBestSocialMoves(int num=1)
    {
        if (num >= socialMoves.Count) return socialMoves;

        List<SocialMove> bestSocialMoves = new List<SocialMove>();

        while(bestSocialMoves.Count < num)
        {
            SocialMove socialMove = socialMoves[Mathf.FloorToInt(Random.value * socialMoves.Count)];
            if (!bestSocialMoves.Contains(socialMove))
            {
                bestSocialMoves.Add(socialMove);
            }
        }

        return bestSocialMoves;
    }

    public DialogueUnit ExpressSocialMove(SocialMove socialMove)
    {
        return new DialogueUnit(socialMove.ToString(), name, socialMove);
    }
    
}
