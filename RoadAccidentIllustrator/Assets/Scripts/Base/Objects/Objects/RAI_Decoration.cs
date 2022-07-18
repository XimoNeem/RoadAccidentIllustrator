using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RAI_Decoration : Movable
{
    [Space]
    public DecorationType subtype;
    public VehicleSettings settingsContainer;

    public override void Start()
    {
        base.Start();

        if (subtype == DecorationType.Растительность && RAI_SettingsManager.instance.currentSettings.vegetationRandom)
        {
            Vector3 newRotation = this.transform.eulerAngles;
            newRotation.y = Random.Range(0, 360);
            this.transform.eulerAngles = newRotation;
            this.transform.localScale *= Random.Range(0.85f, 1.15f);
        }
    }

    public override void ApplySettings()
    {
        base.ApplySettings();


    }
}
