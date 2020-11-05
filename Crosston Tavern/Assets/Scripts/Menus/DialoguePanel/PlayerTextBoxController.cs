using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextBoxController : MenuElement
{
    public GameObject window;
    public Transform buttonHolder;
    public GameObject playerChoiceButtonPrefab;

    BarSpaceController mainMenu;

    public override void Open()
    {
        base.Open();
        window.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        window.SetActive(false);

        foreach (Transform child in buttonHolder) {
            Destroy(child.gameObject);
        }
    }

    public void Initialize(BarSpaceController mm)
    {
        mainMenu = mm;
    }

    public void Fill(List<DialogueUnit> playerChoices)
    {
        foreach(DialogueUnit playerChoice in playerChoices) {
            GameObject button = Instantiate(playerChoiceButtonPrefab, buttonHolder);
            button.GetComponent<PlayerChoiceButton>().LabelButton(playerChoice, mainMenu);
        }

    }
}
