using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class PDDERoadMark : Movable
{
    [Space]
    public RoadMarkType subtype;
    private float offset = 0.01f;
    public Renderer RendererItem;

    public override void Start()
    {
        base.Start();

        this.transform.position += new Vector3(0, offset, 0);
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        
    }

    public void SetPrint(Texture2D sign)
    {
        RendererItem.material.mainTexture = sign;
    }
}
