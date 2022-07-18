using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISettingItemHelper : MonoBehaviour
{
    public TMP_Text label;
    public TMP_Text itemName;
    void Start()
    {
        if (label == null)
        {
            label = this.GetComponentInChildren<TMP_Text>();
        }
    }

    public void SetLabel(string name)
    {
        label.text = name;
    }
}
