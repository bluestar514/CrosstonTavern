using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowniePanel : MonoBehaviour
{
    public Townie townie;
    public Text display;

    public void Set(Townie townie)
    {
        this.townie = townie;

        display.text = this.townie.ToString();
    }
}
