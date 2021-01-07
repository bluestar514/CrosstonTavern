using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainNotebookPanel : MonoBehaviour
{
    public Transform FilterHolder;
    public Transform EventHolder;

    public GameObject filterPrefab;
    public GameObject eventPrefab;
    public GameObject dayPrefab;

    Dictionary<string, DayPanel> dayDict = new Dictionary<string, DayPanel>();
    Dictionary<string, FilterObj> filterDict = new Dictionary<string, FilterObj>();
    Dictionary<string, List<KnownEventPanel>> eventByActorDict = new Dictionary<string, List<KnownEventPanel>>();


    private void Start()
    {
        gameObject.SetActive(false);
    }


    public void AddEvent(WorldFactEvent e)
    {
        WorldTime time = e.action.executionTime;

        if (!dayDict.ContainsKey(time.GetDate())) {
            GameObject o = Instantiate(dayPrefab, EventHolder);
            DayPanel dayPanel = o.GetComponent<DayPanel>();
            dayPanel.Initialize(time, eventPrefab);
            dayDict.Add(time.GetDate(), dayPanel);
        }

        KnownEventPanel panel = dayDict[time.GetDate()].AddEvent(e);

        ExecutedAction action = e.action;
        string actor = action.Action.ActorId;

        if (!filterDict.ContainsKey(actor)) {
            GameObject f= Instantiate(filterPrefab, FilterHolder);
            FilterObj filter = f.GetComponent<FilterObj>();
            filter.Initialize(actor, this);

            filterDict.Add(actor, filter);

            eventByActorDict.Add(actor, new List<KnownEventPanel>());
            
        }

        eventByActorDict[actor].Add(panel);
    }

    public void Close(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void Open(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void UpdateFilters()
    {
        foreach (string actor in filterDict.Keys) {
            foreach (KnownEventPanel panel in eventByActorDict[actor]) {
                panel.gameObject.SetActive(false);
            }
        }

        foreach (string actor in filterDict.Keys) {
            if (filterDict[actor].GetToggleState()) {
                foreach (KnownEventPanel panel in eventByActorDict[actor]) {
                    panel.gameObject.SetActive(true);
                }
            }
        }
    }
}
