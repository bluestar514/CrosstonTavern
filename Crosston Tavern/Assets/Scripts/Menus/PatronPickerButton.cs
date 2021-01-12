using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatronPickerButton : MonoBehaviour
{
    public Text text;

    Townie patron;
    BarSpaceController bsc;

    public void Init(Townie patron, BarSpaceController bsc)
    {
        this.patron = patron;
        this.bsc = bsc;

        text.text = patron.townieInformation.id;

    }

    public void SetPatron()
    {
        bsc.SetPatron(patron);
        bsc.AddConvSeperator();
    }
}
