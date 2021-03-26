using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLoading : MonoBehaviour
{
    public LoadingBar loadingBar;

    public WorldHub hub;

    private void Start()
    {
        loadingBar.SetProgress(.5f);
    }



}
