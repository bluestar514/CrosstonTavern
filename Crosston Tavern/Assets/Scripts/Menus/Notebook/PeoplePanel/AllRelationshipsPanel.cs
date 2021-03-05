using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllRelationshipsPanel : MonoBehaviour
{
    public GameObject relationsPanelPrefab;
    public Transform panelHolder;

    string source;
    Dictionary<string, RelationsPanel> relationPanels = new Dictionary<string, RelationsPanel>();

    public void Init(string townieId)
    {
        source = townieId;
    }

    public void AddRelations(IEnumerable<WorldFact> facts)
    {
        AddRelations(from fact in facts
                     where fact is WorldFactRelation
                     select (WorldFactRelation)fact);
    }

    public void AddRelations(IEnumerable<WorldFactRelation> factRelations)
    {
        AddRelations(from fact in factRelations
                     select fact.relation);
    }

    public void AddRelations(IEnumerable<StateRelation> relations)
    {
        Dictionary<string, List<StateRelation>> sortedRelations = new Dictionary<string, List<StateRelation>>();

        Debug.Log("Adding Relations:");

        foreach(StateRelation relation in relations) {
            Debug.Log(relation);
            

            string partner;
            if(relation.source == source) {
                partner = relation.target;
            } else {
                partner = relation.source;
            }

            //Debug.Log("panel source ==" + source);
            //Debug.Log("relation source ==" + relation.source + " relation target ==" + relation.target);
            //Debug.Log("source == " + source + " partner ==" + partner);

            if (!sortedRelations.ContainsKey(partner))
                sortedRelations.Add(partner, new List<StateRelation>());
            sortedRelations[partner].Add(relation);

        }

        
        foreach(string target in sortedRelations.Keys) {
            
            if (!relationPanels.ContainsKey(target)) {
                Debug.Log("Making Relation Panel for " + source + "->" + target);
                relationPanels.Add(target, Instantiate(relationsPanelPrefab, panelHolder).GetComponent<RelationsPanel>());
                
                relationPanels[target].Init(source, target);
            }

            Debug.Log("Adding Relations to Relation Panel");
            relationPanels[target].UpdateStats(sortedRelations[target]);
        }

    }
    
}
