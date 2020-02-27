using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectPanel : SubDisplayPanel<MicroEffect>
{
    public Text idText;

    MicroEffect effect;

    public override void Set(MicroEffect effect)
    {
        this.effect = effect;

        Display();
    }

    void Display()
    {
        idText.text = effect.ToString();
    }

}
