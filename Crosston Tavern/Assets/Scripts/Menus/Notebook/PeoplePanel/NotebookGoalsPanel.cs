using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookGoalsPanel : MainNotebookTab
{
    public Transform peopleButtonHolder;
    public Transform peopleContentHolder;

    Dictionary<string, PeopleContentPanel> peopleToContentDict = new Dictionary<string, PeopleContentPanel>();

    public GameObject PeopleContentPanelPrefab;
    public GameObject GoalPanelPrefab;
    public GameObject PeopleNameButtonPrefab;


    /// <summary>
    /// Used for adding relationship data at once
    /// </summary>
    /// <param name="facts">Should be WorldFactRelations</param>
    public void AddMany(List<WorldFact> facts)
    {


        Dictionary<string, List<WorldFact>> sortedFacts = new Dictionary<string, List<WorldFact>>();

        foreach(WorldFact fact in facts) {
            Debug.Log(fact);
            if(fact is WorldFactRelation factRelation) {
                if (!sortedFacts.ContainsKey(factRelation.owner))
                    sortedFacts.Add(factRelation.owner, new List<WorldFact>());
                sortedFacts[factRelation.owner].Add(fact);
            }
        }

        foreach(string owner in sortedFacts.Keys) {
            GetPersonContentPanel(owner).allRelationships.AddRelations(sortedFacts[owner]);
        }
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
