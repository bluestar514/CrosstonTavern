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


}
