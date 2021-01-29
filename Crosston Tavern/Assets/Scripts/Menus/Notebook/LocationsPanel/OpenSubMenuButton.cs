using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSubMenuButton : MonoBehaviour
{
    public GameObject subMenu;

    public void OnClick()
    {
        subMenu.SetActive(!subMenu.activeSelf);
    }
}
