using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAI_Unity;

public class RAI_PreviewManager : MonoBehaviour
{
    public static RAI_PreviewManager instance;

    public Transform itemParent;
    public Camera renderCamera;
    public Texture2D text;
    private Vector3 camPosition;

    private void Start()
    {
        instance = this;
        camPosition = renderCamera.transform.position;
    }

    public Texture2D GetPreview(GameObject item)
    {
        renderCamera.transform.position = camPosition;

        Texture2D tex = new Texture2D(512, 512);

        Instantiate(item, itemParent).layer = 15;

        RenderTexture rTex = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
        rTex.Create();

        renderCamera.targetTexture = rTex;
        renderCamera.Render();


        text = ImageManager.toTexture2D(rTex);

        ClearItem();

        return tex;
    }

    void ClearItem()
    {
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }
    }
}
