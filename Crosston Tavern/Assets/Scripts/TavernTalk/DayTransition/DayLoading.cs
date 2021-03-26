using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLoading : MonoBehaviour
{
    public LoadingBar loadingBar;
    public GameObject background;

    public WorldHub hub;

    private void Start()
    {
        loadingBar.SetProgress(.5f);
    }


    public void LoadNextDay()
    {
        StartCoroutine(LoadingScreenAscyn());
    }

    IEnumerator LoadingScreenAscyn()
    {
        background.SetActive(true);

        while(hub.progress < hub.total) {
            loadingBar.SetProgress((float)hub.progress /(float) hub.total);
            yield return new WaitForEndOfFrame();
        }

        background.SetActive(false);
    }
}
