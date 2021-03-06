
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareController : MonoBehaviour
{
    private LensFlare flare;
    private Camera mCamera;
    [Header("Flare brightness")]
    [Range(0, 1)]
    public float multiplier = 1;
    public float ratio = 700;
    [Space]
    public float minFlare = 1;
    public float maxFlare = 150;

    private void Start()
    {
        if (this.GetComponent<LensFlare>())
        {
            flare = this.GetComponent<LensFlare>();
        }
        else
        {
            flare = this.gameObject.AddComponent<LensFlare>();
        }
        mCamera = Camera.main;
        
        if (this.GetComponent<Light>())
        {
            flare.color = this.GetComponent<Light>().color;            
        }

        SetFlareSize();
    }

    private void FixedUpdate()
    {
        if(Input.GetMouseButton(1))
        {
            SetFlareSize();
        }
    }

    private void SetFlareSize()
    {
        if (flare != null)
        {
            flare.brightness = Mathf.Clamp(ratio / Vector3.Distance(this.transform.position, mCamera.transform.position) * multiplier, minFlare, maxFlare);
        }
    }
}
