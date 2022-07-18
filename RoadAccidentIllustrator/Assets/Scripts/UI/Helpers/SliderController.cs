using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    TMP_Text sliderValueText;
    Slider currentSlider;

    private void Start()
    {
        sliderValueText = this.GetComponentInChildren<TMP_Text>();
        currentSlider = this.GetComponentInChildren<Slider>();

        currentSlider.onValueChanged.AddListener(delegate { OnSliderChange(); });

        OnSliderChange();
    }

    private void OnEnable()
    {
        OnSliderChange();
    }

    public void OnSliderChange()
    {
        if (sliderValueText != null)
        {
            string val = currentSlider.value.ToString();
            sliderValueText.text = val.Trim();
        }
    }
}
