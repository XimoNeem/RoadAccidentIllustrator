using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class PDDERoad : Movable
{
    public override void Start()
    {
        base.Start();

    }

    [Space]
    public RoadType subtype;
    [Space]
    public GameObject[] options;
    
}
