using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSettingHelper : MonoBehaviour
{
    public Image ColorImage;

    void Start()
    {
        
    }

    public void SetColor(Color col)
    {
        ColorImage.color = col;
    }
}
