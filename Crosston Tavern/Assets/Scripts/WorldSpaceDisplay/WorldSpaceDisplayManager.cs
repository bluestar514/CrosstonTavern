using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceDisplayManager : MonoBehaviour
{
    public float hPadding;
    public float wPadding;
    float h;
    float w;

    public GameObject PeopleContent;
    public GameObject TimeLineContent;
    public GameObject EventContent;

    public GameObject TownieNamePanelPrefab;
    public GameObject TimePanelPrefab;
    public GameObject EventPanelPrefab;

    List<Dictionary<Townie, EventPanel>> history;

    List<Townie> townieDisplayOrder;

    public void Start()
    {
        h = EventPanelPrefab.GetComponent<RectTransform>().rect.height;
        w = EventPanelPrefab.GetComponent<RectTransform>().rect.width;


        townieDisplayOrder = new List<Townie>() {
            new Townie("Alicia"),
            new Townie("Bob"),
            new Townie("Clara"),
            new Townie("Darel"),
            new Townie("Everet"),
            new Townie("Faraz"),
            new Townie("Gigi"),
            new Townie("Howard")
        };

        history = new List<Dictionary<Townie, EventPanel>>() {
            new Dictionary<Townie, EventPanel>()
        };


        foreach(Townie townie in townieDisplayOrder) {
            AddTownieRow(townie);
        }

        AddTimeStep(0);

        GameObject obj = AddEventPanel(townieDisplayOrder[2], new WorldAction("test"), 0);

        history.Add(new Dictionary<Townie, EventPanel>());
        history[0].Add(townieDisplayOrder[2], obj.GetComponent<EventPanel>());
    }

    GameObject AddTimeStep(int step)
    {
        GameObject timePanel = Instantiate(TimePanelPrefab, TimeLineContent.transform);
        timePanel.GetComponent<TimePanel>().Set(step);

        PositionTimeColumn(timePanel.GetComponent<TimePanel>());
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

    GameObject AddTownieRow(Townie townie)
    {
        GameObject towniePanel = Instantiate(TownieNamePanelPrefab, PeopleContent.transform);
        towniePanel.GetComponent<TowniePanel>().Set(townie);

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

    GameObject AddEventPanel(Townie townie, WorldAction worldAction, int timeStep)
    {
        GameObject eventPanel = Instantiate(EventPanelPrefab, EventContent.transform);
        eventPanel.GetComponent<EventPanel>().Set(townie, worldAction, timeStep);

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

public class Townie
{
    string name;

    public Townie(string name)
    {
        this.name = name;
    }

    public override string ToString()
    {
        return name;
    }
}

