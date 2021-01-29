using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayPanel : MonoBehaviour
{
    public Transform eventHolder;
    public Text dateText;
    Dictionary<string, GameObject> events = new Dictionary<string, GameObject>();
    public GameObject eventPrefab;

    public void Initialize(WorldTime date, GameObject eventPrefab)
    {
        this.eventPrefab = eventPrefab;

        dateText.text = date.GetDate();

        gameObject.name = gameObject.name.Replace("Clone", date.GetDate());
    }

    public KnownEventPanel AddEvent(WorldFactEvent e)
    {
        if (events.ContainsKey(e.ToString())) {
            return null;
        }
        events.Add(e.ToString(), Instantiate(eventPrefab, eventHolder));
        KnownEventPanel panel = events[e.ToString()].GetComponent<KnownEventPanel>();
        panel.Initiate(e);

        return panel;
    }

}
