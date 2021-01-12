using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatronPicker : MonoBehaviour
{
    public WorldHub world;
    public BarSpaceController bsc;
    public Transform PatronButtonHolder;
    public GameObject PatronButtonPrefab;

    List<Townie> allPeople;


    public void OpenPicker()
    {
        if (allPeople == null) {
            allPeople = world.GetTownies();

            foreach (Townie townie in allPeople) {
                if (townie.townieInformation.id == "barkeep") continue;
                GameObject button = Instantiate(PatronButtonPrefab, PatronButtonHolder);
                button.GetComponent<PatronPickerButton>().Init(townie, bsc);
            }
        }

        gameObject.SetActive(true);
    }

}
