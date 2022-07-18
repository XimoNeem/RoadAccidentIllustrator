using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCreator : MonoBehaviour
{
    public Camera renderCamera;
    public RenderTexture renderTexture;
    public LayerMask renderLayer;

    public static PreviewCreator instance;
    private Vector3 camPos;

    private void Start()
    {
        instance = this;
        camPos = renderCamera.transform.position;
    }

    public void CreateTexture(GameObject original, out Texture result)
    {
        renderCamera.transform.position = camPos;

        GameObject item = Instantiate(original, Vector3.zero, Quaternion.identity);
        item.GetComponent<Movable>().enabled = false;
        item.layer = 16;
        foreach (var renderer in item.GetComponentsInChildren<Transform>())
        {
            renderer.gameObject.layer = 16;
        }

        List<float> boundSize = new List<float>();

        if (!item.GetComponent<Collider>())
        {
            result = null;
            return;
        }

        Collider collider = item.GetComponent<Collider>();
        boundSize.Add(collider.bounds.size.x);
        boundSize.Add(collider.bounds.size.y);
        boundSize.Add(collider.bounds.size.z);

        float biggestValue = 0;

        foreach (var value in boundSize)
        {
            if (value > biggestValue)
            {
                biggestValue = value;
            }
        }

        renderCamera.orthographicSize = biggestValue - item.GetComponent<Movable>().previewZoom;
        renderCamera.transform.position += new Vector3(0, item.GetComponent<Movable>().previewVerticalOffset, 0);

        result = Render();

        Destroy(item);
    }

    public Texture Render()
    {
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();


        Texture finalTexture = toTexture2D(renderTexture);

        return finalTexture;
    }

    public Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
