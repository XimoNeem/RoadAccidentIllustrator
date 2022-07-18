using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderSettingItemHelper : MonoBehaviour
{
    void Start()
    {
        if (this.GetComponent<Slider>())
        {
            this.GetComponent<Slider>().onValueChanged.AddListener(delegate { FindObjectOfType<RAI_RenderSettingsManager>().UpdateSettings(); });

        }
        else if (this.GetComponent<Toggle>())
        {
            this.GetComponent<Toggle>().onValueChanged.AddListener(delegate { FindObjectOfType<RAI_RenderSettingsManager>().UpdateSettings(); });
        }

    }
}
