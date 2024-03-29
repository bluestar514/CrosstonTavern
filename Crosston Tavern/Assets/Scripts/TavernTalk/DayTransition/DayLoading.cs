﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLoading : MonoBehaviour
{
    public LoadingBar loadingBar;
    public GameObject background;

    public WorldHub hub;

    public IEnumerator LoadingScreenAscyn()
    {
        Debug.Log("DayLoading: Starting Animation");
        background.SetActive(true);

        while(hub.progress < hub.total || !hub.ready) {
            loadingBar.SetProgress((float)hub.progress /(float) hub.total);
            yield return new WaitForEndOfFrame();
        }

        background.SetActive(false);
        Debug.Log("DayLoading: Ending Animation");
    }
}
