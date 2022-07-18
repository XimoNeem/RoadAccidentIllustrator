using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISizeChanger : MonoBehaviour
{
    public RectTransform ResizeObject;
    public RectTransform ToobBar;
    RectTransform InputTarget;
    public List<Camera> cameras;
    RectTransform mainCanvas;

    [Range(0, 1)]
    public float MinSize;
    [Range(0, 1)]
    public float MaxSize;

    private float minSizeX;
    private float maxSizeX;

    void Start()
    {
        minSizeX = Screen.width * MinSize;
        maxSizeX = Screen.width * MaxSize;

        GameObject target = new GameObject();
        mainCanvas = GameObject.FindGameObjectWithTag("canvas").GetComponent<RectTransform>();
        target.transform.parent = mainCanvas.transform;
        target.name = "_inputTarget";
        InputTarget = target.AddComponent<RectTransform>();
    }

    public void StartDrag()
    {
        InputTarget.position = Input.mousePosition;
    }

    public void OnDrag()
    {
        InputTarget.position = new Vector3(Mathf.Clamp(Input.mousePosition.x, min: minSizeX, max: maxSizeX), Input.mousePosition.y, 0);

        float percentInput = InputTarget.position.x / Screen.width;

        foreach (var item in cameras)
        {
            item.rect = new Rect(percentInput, 0.0f, 1.0f, 1.0f);
        }

        ResizeObject.sizeDelta = new Vector2(InputTarget.position.x - ToobBar.sizeDelta.x, ResizeObject.sizeDelta.y);
    }
}
