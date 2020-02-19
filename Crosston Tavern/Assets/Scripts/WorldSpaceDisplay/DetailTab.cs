using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailTab : MonoBehaviour
{
    public Text displayName;
    public Text location;
    public Text body;

    public bool startOpen = false;


    public void Start()
    {
        gameObject.SetActive(startOpen);
    }

    public void CloseTab()
    {
        gameObject.SetActive(false);
    }
}
