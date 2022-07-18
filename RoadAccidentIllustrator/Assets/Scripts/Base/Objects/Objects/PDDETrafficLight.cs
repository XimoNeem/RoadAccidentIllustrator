using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class PDDETrafficLight : Movable
{
    [Space]
    public TrafficLightType subtype;
    [Space]
    public Renderer upSignal;
    public Renderer centerSignal;
    public Renderer downSignal;
    public Renderer leftSignal;
    public Renderer rightSignal;

    public override void Start()
    {
        base.Start();
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        List<TrafficLightSetting> textures = SettingsManager.GetTrafficLightValues(this);

        foreach (var item in textures)
        {
            //Shader newShader = Shader.Find("Unlit/Texture");


            if (item.lightPosition == trafficLightPosition.up)
            {
                upSignal.material.mainTexture = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture(item.lightType);
                //upSignal.material.shader = newShader;
            }
            else if (item.lightPosition == trafficLightPosition.center)
            {
                centerSignal.material.mainTexture = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture(item.lightType);
                //centerSignal.material.shader = newShader;
            }
            else if (item.lightPosition == trafficLightPosition.down)
            {
                downSignal.material.mainTexture = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture(item.lightType);
                //downSignal.material.shader = newShader;
            }
            else if (item.lightPosition == trafficLightPosition.left)
            {
                leftSignal.material.mainTexture = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture(item.lightType);
                //leftSignal.material.shader = newShader;
            }
            else if (item.lightPosition == trafficLightPosition.right)
            {
                rightSignal.material.mainTexture = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture(item.lightType);
                //rightSignal.material.shader = newShader;
            }
        }
    }
}
