using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maskPortrait : MonoBehaviour
{
    public GameObject mask;
    public GameObject portrait;


    void Start()
    {
        portrait.transform.SetParent(mask.transform);
    }


}
