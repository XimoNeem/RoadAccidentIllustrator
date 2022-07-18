using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class UIListItemHelper : MonoBehaviour
{
    public RAI_ObjectType itemType;


    private void Start()
    {
        if (this.GetComponent<Button>())
        {
            this.GetComponent<Button>().onClick.AddListener(SetType);
        }
    }

    public void SetType()
    {
        if (FindObjectOfType<RAI_ListManager>())
        {
            FindObjectOfType<RAI_ListManager>().SetType(itemType);
        }
    }
}
