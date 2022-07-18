using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RAI_Road : Movable
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
