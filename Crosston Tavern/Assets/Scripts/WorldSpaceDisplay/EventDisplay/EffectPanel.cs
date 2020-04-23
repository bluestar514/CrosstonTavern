using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectPanel : SubDisplayPanel<Effect>
{
    public Text idText;

    Effect effect;

    public override void Set(Effect effect)
    {
        this.effect = effect;

        Display();
    }

    void Display()
    {
        idText.text = effect.ToString();
    }

}
