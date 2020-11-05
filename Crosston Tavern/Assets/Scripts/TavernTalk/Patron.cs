using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Patron 
{
    string name;
    public string Name { get => name; set => name = value; }

    public Townie townie;
    public List<SocialMove> socialMoves;

    ConversationController cc;

    public Patron(Townie townie, List<SocialMove> socialMoves)
    {
        this.townie = townie;
        this.Name = townie.townieInformation.id;

        this.socialMoves = socialMoves; 
        cc = new ConversationController(this);
    }

    public SocialMove PickSocialMove(SocialMove prompt)
    {
        return cc.GiveResponse(prompt); //PickBestSocialMoves(1)[0];
    }

    public List<SocialMove> GetSocialMoves(Townie partner)
    {
        List<SocialMove> moves = new List<SocialMove>(socialMoves);
        moves.AddRange(GenAskWhyGoal(partner.gm.lastSetOfGoals));
        moves.AddRange(GenAskWhyAction(partner.ws.knownFacts.GetHistory()));
        return moves;
    }

    public DialogueUnit ExpressSocialMove(SocialMove socialMove)
    {
        return new DialogueUnit(socialMove.ToString(), name, socialMove);
    }


    List<SocialMove> GenAskWhyGoal(List<Goal> goals)
    {
        return new List<SocialMove>(from goal in goals
                                    select new SocialMove("askWhyGoal#", new List<string>() { goal.name }));
    }

    List<SocialMove> GenAskWhyAction(List<ExecutedAction> actions)
    {
        return new List<SocialMove>(from action in actions
                                    select new SocialMove("askWhyAction#", new List<string>() { action.ToString() }));
    }
}
