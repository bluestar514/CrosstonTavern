using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDeactivated : MonoBehaviour
{
    public bool startActivated = true;

    void Start()
    {
        gameObject.SetActive(startActivated);
    }

}
