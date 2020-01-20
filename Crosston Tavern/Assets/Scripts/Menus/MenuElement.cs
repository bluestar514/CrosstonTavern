using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuElement : MonoBehaviour
{
    public virtual void Open()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        this.gameObject.SetActive(false);
    }

}
