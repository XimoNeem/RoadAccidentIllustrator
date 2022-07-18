using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class PDDEVehicle : Movable
{
    [Space]
    public VehicleType subtype;
    public VehicleSettings settingsContainer;
    [Space]
    public GameObject LeftTurn;
    public GameObject RightTurn;
    public GameObject ForwardLight;
    public GameObject FarForwardLight;
    public GameObject StopLight;
    public Color corpusColor;

    public Renderer corpus;

    public Renderer[] stopLights;
    public Renderer[] lights;
    [Space]
    public Shader defaultShader;
    public Shader lightShader;

    public override void Start()
    {
        base.Start();
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        LeftTurn.SetActive(SettingsManager.GetBoolValue(this, settingItemType.vehicle_leftturn));
        RightTurn.SetActive(SettingsManager.GetBoolValue(this, settingItemType.vehicle_rightturn));
        ForwardLight.SetActive(SettingsManager.GetBoolValue(this, settingItemType.vehicle_lights));
        FarForwardLight.SetActive(SettingsManager.GetBoolValue(this, settingItemType.vehicle_farlights));
        StopLight.SetActive(SettingsManager.GetBoolValue(this, settingItemType.vehicle_stoplights));
        corpusColor = (SettingsManager.GetColorValue(this, settingItemType.vehicle_color));

        SetLightShader(SettingsManager.GetBoolValue(this, settingItemType.vehicle_lights), lights);
        SetLightShader(SettingsManager.GetBoolValue(this, settingItemType.vehicle_farlights), lights);
        SetLightShader(SettingsManager.GetBoolValue(this, settingItemType.vehicle_stoplights), stopLights);

        corpusColor = SettingsManager.GetColorValue(this, settingItemType.vehicle_color);

        SetColor(corpusColor);
    }

    private void SetLightShader(bool state, Renderer[] list)
    {
        foreach (var item in list)
        {
            if (state)
            {
                item.material.shader = lightShader;
            }
            else
            {
                item.material.shader = defaultShader;
            }
        }
    }


    private void SetColor(Color color)
    {
        foreach (var item in objectSettings)
        {
            if (item.type == settingItemType.vehicle_color)
            {
                ColorContainer cont = (ColorContainer)item;
                cont.value = color;
            }
        }

        corpusColor = color;
        corpus.material.color = corpusColor;
    }
}
