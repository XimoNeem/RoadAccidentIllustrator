using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RoadAccidentIllustrator.RAI_UI;

public class ToolbarItemHelper : MonoBehaviour
{
    public ToolbarItemType itemType;

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SetCurrentItem);
    }

    public void SetCurrentItem()
    {
        FindObjectOfType<MainUIManager>().SetToolbarItem(itemType);
    }
}
