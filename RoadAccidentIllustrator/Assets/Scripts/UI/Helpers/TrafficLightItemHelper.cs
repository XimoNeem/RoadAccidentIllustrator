using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class TrafficLightItemHelper : MonoBehaviour
{
    public trafficLightSignalType signalType;
    public trafficLightPosition lightPosition;

    public int signalValue;

    void Start()
    {
        Texture2D tex = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture((trafficLightSignalType)signalValue);
        this.GetComponentInChildren<RawImage>().texture = tex;
    }

    public void ChangeSignal()
    {
        signalValue += 1;

        string[] values = System.Enum.GetNames(signalType.GetType());

        if (signalValue == values.Length)
        {
            signalValue = 0;
            Debug.Log("Yt cdtnbncz");
        }
        else
        {
            Debug.Log("Светиться");
        }

        signalType = (trafficLightSignalType)signalValue;

        Texture2D tex = FindObjectOfType<RAI_TrafficLightSignManager>().GetSignalTexture((trafficLightSignalType)signalValue);

        if (ObjectSettingsManager.instance.currentItem is PDDETrafficLight)
        {
            PDDETrafficLight tl = (PDDETrafficLight)ObjectSettingsManager.instance.currentItem;
            TrafficLightContainer cont = (TrafficLightContainer)tl.objectSettings[0];

            foreach (var item in cont.textures)
            {
                if (item.lightPosition == this.lightPosition)
                {
                    item.lightType = signalType;
                }
            }

            this.GetComponentInChildren<RawImage>().texture = tex;
            tl.ApplySettings();
        }
        else
        {
            RAI_DebugManager.instance.ShowMessage("WRONG OBJECT", Color.red);
        }
    }
}
