using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class TrafficLightGroupHelper : MonoBehaviour
{
    public trafficLightSettingType groupType;
    
    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
