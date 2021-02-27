using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarSpaceControllerDebug : BarSpaceController
{
    public override void Start()
    {
        StartCoroutine(DoWhenReady());
    }

    IEnumerator DoWhenReady()
    {
        yield return new WaitUntil(() => worldHub.ready);
        Init();

    }
}
