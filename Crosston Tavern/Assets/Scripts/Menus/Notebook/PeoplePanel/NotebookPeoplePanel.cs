using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NotebookPeoplePanel : MainNotebookTab
{
    public Transform peopleButtonHolder;
    public Transform peopleContentHolder;

    Dictionary<string, PeopleContentPanel> peopleToContentDict = new Dictionary<string, PeopleContentPanel>();

    public GameObject PeopleContentPanelPrefab;
    public GameObject GoalPanelPrefab;
    public GameObject PeopleNameButtonPrefab;


    /// <summary>
    /// Used for adding relationship data at once
    /// And marking all goals we are having trouble with
    /// </summary>
    /// <param name="facts">Should be WorldFactRelations</param>
    public void AddMany(List<WorldFact> facts)
    {
        Dictionary<string, List<WorldFact>> sortedFacts = new Dictionary<string, List<WorldFact>>();

        foreach (WorldFact fact in facts) {
            string owner = "";

            if (fact is WorldFactRelation factRelation) {
                owner = factRelation.owner;
            }
            if(fact is WorldFactGoal goalFact) {
                owner = goalFact.owner;
            }


            if (owner != "") {
                if (!sortedFacts.ContainsKey(owner))
                    sortedFacts.Add(owner, new List<WorldFact>());
                sortedFacts[owner].Add(fact);
            }
        }


        UpdateRelations(sortedFacts);
        UpdateStuckGoals(sortedFacts);
        UpdatePlayerDefinedGoals(sortedFacts);
    }


    void UpdateRelations(Dictionary<string, List<WorldFact>> sortedFacts)
    {
        foreach (string owner in sortedFacts.Keys) {
            GetPersonContentPanel(owner).allRelationships.AddRelations(sortedFacts[owner]);
        }
    }

    void UpdateStuckGoals(Dictionary<string, List<WorldFact>> sortedFacts)
    {
        foreach (string owner in sortedFacts.Keys) {
            if (ContainsMarkedGoal(sortedFacts[owner], "stuck")){
                GetPersonContentPanel(owner).MarkStuckGoals(sortedFacts[owner]);
            }
        }
    }
    void UpdatePlayerDefinedGoals(Dictionary<string, List<WorldFact>> sortedFacts)
    {
        foreach (string owner in sortedFacts.Keys) {
            if (ContainsMarkedGoal(sortedFacts[owner], "player")) {
                GetPersonContentPanel(owner).MarkPlayerSpecifiedGoals(sortedFacts[owner]);
            }
        }
    }
    bool ContainsMarkedGoal(List<WorldFact> facts, string mark)
    {
        return facts.Any(fact => {
            if (fact is WorldFactGoal factGoal) {
                return factGoal.modifier.Contains(mark);
            } else return false;
        });
    }

    public override bool AddWorldFact(WorldFact fact)
    {
        PeopleContentPanel panel = GetPanelFromFact(fact);
        if (panel == null) return false;
        return panel.AddWorldFact(fact);

    }

    public override bool RemoveWorldFact(WorldFact fact)
    {
        PeopleContentPanel panel = GetPanelFromFact(fact);
        if (panel == null) return false;

        return panel.RemoveWorldFact(fact);
    }

    PeopleContentPanel GetPanelFromFact(WorldFact fact)
    {
        string owner;

        if (fact is WorldFactGoal) {
            WorldFactGoal goalFact = (WorldFactGoal)fact;
            owner = goalFact.owner;
        } else if (fact is WorldFactPreference) {
            WorldFactPreference preferenceFact = (WorldFactPreference)fact;
            owner = preferenceFact.person;
        } else {
            return null;
        }

         return GetPersonContentPanel(owner);
    }

    PeopleContentPanel GetPersonContentPanel(string person)
    {
        if (peopleToContentDict.ContainsKey(person)) {
            return peopleToContentDict[person];
        } else {
            PeopleContentPanel panel = Instantiate(PeopleContentPanelPrefab, peopleContentHolder).GetComponent<PeopleContentPanel>();
            panel.Init(person, GoalPanelPrefab);
            peopleToContentDict.Add(person, panel);

            NotebookButton button = Instantiate(PeopleNameButtonPrefab, peopleButtonHolder).GetComponent<NotebookButton>();
            button.Init(person, this);


            return panel;
        }
    }

    public override void OpenSubTab(string person)
    {
        foreach(PeopleContentPanel panel in peopleToContentDict.Values) {
            panel.gameObject.SetActive(false);
        }

        peopleToContentDict[person].gameObject.SetActive(true);
    }

}
