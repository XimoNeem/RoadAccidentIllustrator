using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoadAccidentIllustrator.RAI_Settings;
using UnityEngine.Rendering.PostProcessing;

public class RAI_RenderSettingsManager : MonoBehaviour
{
    [SerializeField]
    private RAI_RenderSettingsContainer currentRenderSettings;

    public float isoViewSizeMultiplayer;
    public float isoViewSizeMin;
    public float isoViewSizeMax;

    #region UI
    public Toggle fxToggle;
    public Toggle perspectiveToggle;
    public Slider brightnessSlider;
    public Slider contrastSlider;
    public Slider occlusionSlider;
    public Slider saturationSlider;
    public Slider temperatureSlider;
    public Slider viewSizeSlider;
    [Space]
    public RawImage previewImage;
    public RenderTexture renderTex;
    #endregion UI

    #region ControlItems

    public Camera currentRenderCamera;
    public PostProcessVolume postPro;

    #endregion ControlItems

    void Start()
    {
        currentRenderSettings = new RAI_RenderSettingsContainer();
        SetSettingsUI();

        currentRenderCamera.Render();
    }

    private IEnumerator ApplySettings()
    {
        //SetCameraProjection(currentRenderSettings.perspective);

        currentRenderCamera.orthographic = currentRenderSettings.isometric;
        SetViewSize(currentRenderSettings.viewSize);
        postPro.isGlobal = currentRenderSettings.fxEnabled;
        postPro.sharedProfile.GetSetting<ColorGrading>().brightness.value = currentRenderSettings.brightness;
        postPro.sharedProfile.GetSetting<ColorGrading>().contrast.value = currentRenderSettings.contrast;
        postPro.sharedProfile.GetSetting<AmbientOcclusion>().intensity.value = currentRenderSettings.occlusion;
        postPro.sharedProfile.GetSetting<ColorGrading>().saturation.value = currentRenderSettings.saturation;
        postPro.sharedProfile.GetSetting<ColorGrading>().temperature.value = currentRenderSettings.temperature;

        yield return new WaitForEndOfFrame();

        currentRenderCamera.Render();
    }

    private void SetSettingsUI()
    {
        fxToggle.isOn = currentRenderSettings.fxEnabled;
        perspectiveToggle.isOn = currentRenderSettings.isometric;
        brightnessSlider.value = currentRenderSettings.brightness;
        contrastSlider.value = currentRenderSettings.contrast;
        occlusionSlider.value = currentRenderSettings.occlusion;
        saturationSlider.value = currentRenderSettings.saturation;
        temperatureSlider.value = currentRenderSettings.temperature;
        viewSizeSlider.value = currentRenderSettings.viewSize;

    }

    public void UpdateSettings()
    {
        currentRenderSettings.fxEnabled = fxToggle.isOn;
        currentRenderSettings.isometric = perspectiveToggle.isOn;
        currentRenderSettings.brightness = brightnessSlider.value;
        currentRenderSettings.contrast = contrastSlider.value;
        currentRenderSettings.occlusion = occlusionSlider.value;
        currentRenderSettings.saturation = saturationSlider.value;
        currentRenderSettings.temperature = temperatureSlider.value;
        currentRenderSettings.viewSize = viewSizeSlider.value;

        StartCoroutine(ApplySettings());
    }

    private void SetCameraProjection(bool state)
    {
        currentRenderCamera.orthographic = !state;

        if (currentRenderCamera.orthographic)
        {
            currentRenderSettings.viewSize = 5;
        }
        else
        {
            currentRenderSettings.viewSize = 60;
        }

        SetSettingsUI();
    }

    private void SetViewSize(float value)
    {
        if (currentRenderCamera.orthographic)
        {
            currentRenderCamera.orthographicSize = Mathf.Clamp(value + isoViewSizeMultiplayer, isoViewSizeMin, isoViewSizeMax);
        }
        else
        {
            currentRenderCamera.fieldOfView = value;
        }
    }
}
