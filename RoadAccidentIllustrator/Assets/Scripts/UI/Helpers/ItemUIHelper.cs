using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUIHelper : MonoBehaviour
{
    public TMP_Text label;
    public RawImage preview;
    [Space]
    public bool isAdding;
    Button currentButton;
    public GameObject original;
    public string objectName;

    private void Start()
    {
        currentButton = this.GetComponentInChildren<Button>();
    }

    public void Click()
    {
        if (!isAdding)
        {
            currentButton.GetComponentInChildren<TMP_Text>().text = "Отмена";
            currentButton.GetComponent<Button>().colors = RAI_ColorsManager.instance.cancelButton;
            isAdding = true;

            ItemCreator.instance.SetOriginal(original);
            ItemCreator.instance.SetCreationState(true);

            SetOtherButtonsState(false);
        }
        else if (isAdding)
        {
            currentButton.GetComponentInChildren<TMP_Text>().text = "Добавить";
            currentButton.GetComponent<Button>().colors = RAI_ColorsManager.instance.normalButton;
            isAdding = false;

            ItemCreator.instance.SetCreationState(false);

            SetOtherButtonsState(true);
        }
    }

    private void SetOtherButtonsState(bool state)
    {
        foreach (var item in FindObjectsOfType<ItemUIHelper>())
        {
            if (item != this)
            {
                item.GetComponentInChildren<Button>().interactable = state;
            }
        }
    }

    public void CreateItem(Movable item)
    {
        if (item.objectName != "")
        {
            objectName = label.text = item.objectName;
        }
        else
        {
            objectName = label.text = item.name;
        }

        original = item.gameObject;
    }
}
