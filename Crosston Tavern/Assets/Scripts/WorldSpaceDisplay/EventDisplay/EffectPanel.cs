using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectPanel : MonoBehaviour
{
    public Text idText;

    MicroEffect effect;

    public void Set(MicroEffect effect)
    {
        this.effect = effect;

        Display();
    }

    void Display()
    {
        idText.text = effect.ToString();
    }

}
