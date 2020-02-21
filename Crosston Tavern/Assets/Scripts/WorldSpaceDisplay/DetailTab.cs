using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailTab : MonoBehaviour
{
    public Text displayName;

    public bool startOpen = false;


    public void Start()
    {
        gameObject.SetActive(startOpen);
    }

    public void OpenTab()
    {

        gameObject.SetActive(true);
    }

    public virtual void CloseTab()
    {
        gameObject.SetActive(false);
    }
}
