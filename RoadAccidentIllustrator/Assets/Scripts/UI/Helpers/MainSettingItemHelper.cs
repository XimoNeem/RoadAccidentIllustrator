using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoadAccidentIllustrator.RAI_Settings;

public class MainSettingItemHelper : MonoBehaviour
{
    public SettingItemType settingType;



    private void Awake()
    {
        FindObjectOfType<RAI_SettingsManager>().helpers.Add(this);

        if (this.GetComponent<Toggle>())
        {
            Toggle toggle = this.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate { FindObjectOfType<RAI_SettingsManager>().UpadateSettings(); });
        }
        else if (this.GetComponent<Slider>())
        {
            Slider slider = this.GetComponent<Slider>();
            slider.onValueChanged.AddListener(delegate { FindObjectOfType<RAI_SettingsManager>().UpadateSettings(); });
        }
    }

    public float GetValue()
    {
        if (this.GetComponent<Slider>())
        {
            Slider slider = this.GetComponent<Slider>();
            return slider.value;
        }

        return 0;
    }

    public bool GetState()
    {
        if (this.GetComponent<Toggle>())
        {
            Toggle toggle = this.GetComponent<Toggle>();
            return toggle.isOn;
        }
        return false;
    }

    public void SetValue(float value)
    {
        if (this.GetComponent<Slider>())
        {
            Slider slider = this.GetComponent<Slider>();
            slider.value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
        }
    }

    public void SetValue(bool value)
    {
        if (this.GetComponent<Toggle>())
        {
            Toggle toggle = this.GetComponent<Toggle>();
            toggle.isOn = value;
        }
    }
}
