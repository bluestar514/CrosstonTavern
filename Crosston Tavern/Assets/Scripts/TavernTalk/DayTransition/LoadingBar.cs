using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public GameObject progress;
    public GameObject background;
    float percentage;

    public void SetProgress(float percent)
    {
        percentage = percent;

        Image progressBar = progress.GetComponent<Image>();

        progressBar.fillAmount = percentage;
    }
}
