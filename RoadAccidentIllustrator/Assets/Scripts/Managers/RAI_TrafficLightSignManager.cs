using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RAI_TrafficLightSignManager : MonoBehaviour
{
    public static RAI_TrafficLightSignManager instance;
    public List<TrafficSignal> trafficSignals;

    private void Start()
    {
        instance = this;
    }

    public Texture2D GetSignalTexture(trafficLightSignalType type)
    {
        foreach (var item in trafficSignals)
        {
            if (item.signalType == type)
            {
                return item.tex;
            }
        }
        throw new RoadAccidentIllustrator.RAI_Exceptions.RAI_Exception.NoSignalException("no signal");
    }
}

[System.Serializable]
public class TrafficSignal
{
    public trafficLightSignalType signalType;
    public Texture2D tex;
}