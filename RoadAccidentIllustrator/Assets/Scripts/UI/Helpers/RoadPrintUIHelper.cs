using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RoadPrintUIHelper : MonoBehaviour
{
    public Texture2D currentTex;
    public void SetPrint()
    {
        if (ObjectSettingsManager.instance.currentItem is PDDERoadMark)
        {
            PDDERoadMark sign = (PDDERoadMark)FindObjectOfType<ObjectSettingsManager>().currentItem;

            sign.SetPrint(currentTex);

            TextureContainer cont = (TextureContainer)sign.objectSettings[0]; cont.texture = currentTex;

            FindObjectOfType<ObjectSettingsManager>().SetSettingsManager();
            FindObjectOfType<RoadPrintListController>().HideList();
        }
        else
        {
            throw new RoadAccidentIllustrator.RAI_Exceptions.RAI_Exception.ErrorException("NO OBJECT SELECTED");
        }
    }
}
