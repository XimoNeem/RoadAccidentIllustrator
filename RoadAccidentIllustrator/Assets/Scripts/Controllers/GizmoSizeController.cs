using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSizeController : MonoBehaviour
{
    //private Transform mainCamera;
    public float minDist = 20;
    public float maxDist = 80;

    private void Start()
    {
        //mainCamera = Camera.main.transform;
    }
    private void Update()
    {
        if(Input.GetMouseButton(1) | Input.GetMouseButton(0))
        {
            float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
            if(dist < minDist)
            {
                transform.localScale = Vector3.one;
            }
            else if(dist > minDist && dist < maxDist)
            {
                this.transform.localScale = Vector3.one * Vector3.Distance(transform.position, Camera.main.transform.position) * 0.05f;   
            }
            else if(dist > maxDist)
            {
                this.transform.localScale = Vector3.one * maxDist * 0.05f;
            }
        }
    }
}
