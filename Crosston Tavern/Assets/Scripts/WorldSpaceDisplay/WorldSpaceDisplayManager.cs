using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceDisplayManager : MonoBehaviour
{
    public float hPadding;
    public float wPadding;
    float h;
    float w;

    public GameObject PeopleContent;
    public GameObject TimeLineContent;
    public GameObject EventContent;

    public PeopleDetailTab PeopleDetailTab;

    public GameObject TownieNamePanelPrefab;
    public GameObject TimePanelPrefab;
    public GameObject EventPanelPrefab;

    List<Dictionary<string, EventPanel>> history;

    List<string> townieDisplayOrder;

    public void Start()
    {
        h = EventPanelPrefab.GetComponent<RectTransform>().rect.height;
        w = EventPanelPrefab.GetComponent<RectTransform>().rect.width;


        townieDisplayOrder = new List<string>() {};

        history = new List<Dictionary<string, EventPanel>>() {};
    }

    public void AddPeople(List<Person> people)
    {
        townieDisplayOrder = new List<string>(from person in people
                                                select person.Id);

        foreach(Person person in people) {
            AddTownieRow(person);
        }
    }

    public void AddEvent(ExecutedAction action, int timeStep)
    {
        if (history.Count <= timeStep) AddTimeStep(timeStep);

        string townie = action.Action.ActorId;

        GameObject obj = AddEventPanel(townie, action, timeStep);

        history[timeStep].Add(townie, obj.GetComponent<EventPanel>());
    }

    GameObject AddTimeStep(int step)
    {
        GameObject timePanel = null;
        while (history.Count <= step) {
            timePanel = Instantiate(TimePanelPrefab, TimeLineContent.transform);
            timePanel.GetComponent<TimePanel>().Set(history.Count);

            PositionTimeColumn(timePanel.GetComponent<TimePanel>());

            history.Add(new Dictionary<string, EventPanel>());
        }

        ResizeContentPanels();
        return timePanel;
    }
    void PositionTimeColumn(TimePanel panel)
    {
        float y = TimeLineContent.GetComponent<RectTransform>().rect.height / 2;
        float x = w / 2;

        float panelX = panel.timeStep * (w + wPadding) +x;

        panel.GetComponent<RectTransform>().localPosition = new Vector3(panelX, -y);
    }

    GameObject AddTownieRow(Person townie)
    {
        GameObject towniePanel = Instantiate(TownieNamePanelPrefab, PeopleContent.transform);
        towniePanel.GetComponent<TowniePanel>().Set(townie, PeopleDetailTab);

        PositionTownieRow(towniePanel.GetComponent<TowniePanel>());
        ResizeContentPanels();

        return towniePanel;
    }
    void PositionTownieRow(TowniePanel panel)
    {
        float y = h / 2;
        float x = PeopleContent.GetComponent<RectTransform>().rect.width / 2;

        float panelY = townieDisplayOrder.IndexOf(panel.townie) * (h + hPadding) + y;
        
        panel.GetComponent<RectTransform>().localPosition = new Vector3(x, -panelY);
    }

    GameObject AddEventPanel(string townie, ExecutedAction action, int timeStep)
    {
        GameObject eventPanel = Instantiate(EventPanelPrefab, EventContent.transform);
        eventPanel.GetComponent<EventPanel>().Set(townie, action, timeStep);

        PositionEventPanel(eventPanel.GetComponent<EventPanel>());
        ResizeContentPanels();

        return eventPanel;
    }
    void PositionEventPanel(EventPanel panel)
    {
        float y = h / 2;
        float x = w / 2;

        float panelY = townieDisplayOrder.IndexOf(panel.townieRow) * (h+hPadding) + y;
        float panelX = panel.timeStep * (w + wPadding) + x;

        panel.GetComponent<RectTransform>().localPosition = new Vector3(panelX, -panelY);
    }

    void ResizeContentPanels()
    {
        float width = (w + wPadding) * (history.Count);
        float height = (h + hPadding) * (townieDisplayOrder.Count);

        EventContent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        float pWidth = PeopleContent.GetComponent<RectTransform>().rect.width;
        PeopleContent.GetComponent<RectTransform>().sizeDelta = new Vector2(pWidth, height);

        float tHeight = TimeLineContent.GetComponent<RectTransform>().rect.height;
        TimeLineContent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, tHeight);
    }
}

