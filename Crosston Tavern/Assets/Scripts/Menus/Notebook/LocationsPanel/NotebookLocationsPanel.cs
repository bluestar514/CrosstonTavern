using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookLocationsPanel : MainNotebookTab
{
    public Transform locationSelectorHolder;
    public Transform featureHolder;

    public GameObject locationSelectorPanelPrefab;
    public GameObject featureButtonPrefab;

    public GameObject featureContentPanel;

    Dictionary<string, LocationSelectorPanel> locationSelectorDict = new Dictionary<string, LocationSelectorPanel>();
    Dictionary<string, FeatureContentPanel> featurePanelDict = new Dictionary<string, FeatureContentPanel>();

    void Start()
    {
        Dictionary<string, Feature> featureDict = FeatureInitializer.GetAllFeatures();

        foreach (Feature feature in featureDict.Values) {
            if (feature.type == Feature.FeatureType.SYSTEM) continue;
            if (feature.type == Feature.FeatureType.person) continue;
            if (feature.type == Feature.FeatureType.door) continue;
            AddFeature(feature);
        }
    }


    public override bool AddWorldFact(WorldFact fact)
    {
        Debug.Log(fact);

        if(fact is WorldFactResource) {
            WorldFactResource resourceFact = (WorldFactResource)fact;
            featurePanelDict[resourceFact.featureId].AddItem(resourceFact.potentialBinding);

            return true;
        }

        return base.AddWorldFact(fact);
    }

    public override bool RemoveWorldFact(WorldFact fact)
    {
        if (fact is WorldFactResource) Debug.LogWarning("Trying to remove WorldFactResource (" + fact + "). This is probably not desired and will result in unexpected behavior!");

        return base.RemoveWorldFact(fact);
    }

    void AddFeature(Feature feature)
    {
        if( !locationSelectorDict.ContainsKey(feature.location)) {
            LocationSelectorPanel newPanel = Instantiate(locationSelectorPanelPrefab,
                                                    locationSelectorHolder).GetComponent<LocationSelectorPanel>();
            newPanel.Init(this, featureButtonPrefab);

            locationSelectorDict.Add(feature.location, newPanel);
        }

        LocationSelectorPanel selectorPanel = locationSelectorDict[feature.location];
        selectorPanel.AddFeature(feature.id);

        FeatureContentPanel contentPanel = Instantiate(featureContentPanel, featureHolder).GetComponent<FeatureContentPanel>();
        contentPanel.Init(feature);

        featurePanelDict.Add(feature.id, contentPanel);

    }

    public override void OpenSubTab(string str)
    {
        foreach(FeatureContentPanel panel in featurePanelDict.Values) {
            panel.gameObject.SetActive(false);
        }

        featurePanelDict[str].gameObject.SetActive(true);
    }
}
