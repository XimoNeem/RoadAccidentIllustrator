using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RoadSignUIHelper : MonoBehaviour
{
    public Texture2D currentTex;
    public void SetSign()
    {
        if (FindObjectOfType<ObjectSettingsManager>().currentItem is PDDETrafficSign)
        {
            PDDETrafficSign sign = (PDDETrafficSign)FindObjectOfType<ObjectSettingsManager>().currentItem;

            sign.SetSign(currentTex);

            RoadSignContainer cont = (RoadSignContainer)sign.objectSettings[0]; cont.texture = currentTex;

            FindObjectOfType<ObjectSettingsManager>().SetSettingsManager();
            FindObjectOfType<TrafficSignListController>().HideList();
        }
    }
}
