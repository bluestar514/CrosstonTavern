using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationSelectorPanel : MonoBehaviour
{
    public Text mainLocationName;
    public Transform featureHolder;
    public MainNotebookTab mainNotebook;

    public GameObject featureButtonPrefab;

    public void Init(MainNotebookTab mainNotebook, GameObject featureButtonPrefab, string location)
    {
        this.mainNotebook = mainNotebook;
        this.featureButtonPrefab = featureButtonPrefab;


        mainLocationName.text = VerbalizationDictionary.CapFirstLetter(
                                VerbalizationDictionary.Replace(location));
    }

    public void AddFeature(string featureId)
    {
        NotebookButton button = Instantiate(featureButtonPrefab, featureHolder).GetComponent<NotebookButton>();

        button.Init(featureId, mainNotebook);

        
    }

}
