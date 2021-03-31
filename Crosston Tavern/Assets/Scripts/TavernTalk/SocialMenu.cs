using System.Collections.Generic;

[System.Serializable]
public class SocialMenu: SocialMove
{
    public List<SocialMove> menuOptions;
    public SocialMove previousContext;

    public SocialMenu(string verb, List<SocialMove> menuOptions, SocialMove previousContext = null, List<string> arguements = null,
                         List<WorldFact> mentionedFacts = null, List<WorldFact> retractedFacts = null): 
        base(verb, arguements, mentionedFacts, retractedFacts)
    {
        this.menuOptions = menuOptions;

        this.previousContext = previousContext;
        if(previousContext != null) {
            this.menuOptions.Add(previousContext);
        }
        
    }

    public void AddPreviousContext(SocialMove previousContext)
    {
        if(this.previousContext!= null) {
            menuOptions.RemoveAt(menuOptions.Count - 1);
        }

        this.previousContext = previousContext;
        menuOptions.Add(previousContext);
        
    }
}