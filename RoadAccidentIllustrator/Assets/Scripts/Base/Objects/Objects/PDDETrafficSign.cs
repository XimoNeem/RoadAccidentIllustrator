using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class PDDETrafficSign : Movable
{
    [Space]
    public TrafficSignType subtype;
    //public VehicleSettings settingsContainer;
    [Space]
    public Renderer SignItem;

    public override void Start()
    {
        base.Start();
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        SignItem.material.mainTexture = SettingsManager.GetTrafficSignValue(this, settingItemType.roadsign_sign);
    }

    public void SetSign(Texture2D sign)
    {
        SignItem.material.mainTexture = sign;
    }
}
