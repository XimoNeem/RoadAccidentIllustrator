using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RAI_ColorPickerManager : MonoBehaviour
{
    public ObjectMover mover;
    public Color currentColor;
    public float CurrentAlpha;
    public static RAI_ColorPickerManager instance;

    public Image MainHandle;
    public GameObject WindowObject;
    public Button SaveButton;
    public Slider ColorSlider;
    public Image ColorBackground;
    public Image ColorHandle;
    public Slider AlphaSlider;
    public Image AlphaHandle;
    public RawImage ColorImage;
    public RectTransform ColorImageMask;

    private int handleMoveOffset = 5;
    public delegate void setColorLink(SettingContainer currentSetting, Color newValue);

    void Awake()
    {
        instance = this;
        mover = FindObjectOfType<ObjectMover>();
    }

    public void StartManager(setColorLink link, SettingContainer currentSetting)
    {
        mover.SetEditable(false);

        WindowObject.SetActive(true);
        SetColorImage(Color.red);

        AlphaHandle.color = Color.Lerp(Color.black, Color.white, Mathf.Clamp(0, 0.2f, 1f));
        ColorHandle.color = Color.Lerp(Color.white, Color.red, 0.7f);
        MainHandle.GetComponent<RectTransform>().anchoredPosition = ColorImageMask.sizeDelta * 0.9f;
        ColorSlider.maxValue = ColorBackground.sprite.texture.width;

        GetColor();

        SaveButton.onClick.RemoveAllListeners();
        SaveButton.onClick.AddListener(delegate { link(currentSetting, currentColor); } );
        SaveButton.onClick.AddListener(delegate { WindowObject.SetActive(false); });
        SaveButton.onClick.AddListener(delegate { mover.SetEditable(true); });

    }

    private void SetColorImage(Color currentColor)
    {
        Texture2D tex = new Texture2D(100, 50, TextureFormat.RGBA32, true, false);

        int height = tex.height;
        int width = tex.width;

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                tex.SetPixel(y, x, Color.Lerp(Color.white, currentColor, (float)y / (float)width));
                tex.SetPixel(y, x, Color.Lerp(tex.GetPixel(y, x), Color.black, 1 - (float)x / (float)height));
                
            }
        }
        tex.Apply();

        ColorImage.texture = tex;
    }

    public void SetColor(float value)
    {
        int wigth = ColorBackground.sprite.texture.width;
        Color col = ColorBackground.sprite.texture.GetPixel((int)value, ColorBackground.sprite.texture.height / 2);

        ColorHandle.color = Color.Lerp(Color.white, col, 0.7f);

        SetColorImage(col);
        GetColor();
    }

    public void SetAlpha(float value)
    {
        AlphaHandle.color = Color.Lerp(Color.black, Color.white, Mathf.Clamp(value, 0.2f, 1f));

        CurrentAlpha = value;
    }

    public void SetHandlePosition()
    {
        Vector2 mPos = Input.mousePosition;
        MainHandle.transform.position = mPos;
        Vector2 windowSize = ColorImageMask.sizeDelta;
        RectTransform handle = MainHandle.GetComponent<RectTransform>();

        Vector2 newPos = handle.anchoredPosition;
        newPos.x = Mathf.Clamp(newPos.x, 0 + handleMoveOffset, windowSize.x - handleMoveOffset);
        newPos.y = Mathf.Clamp(newPos.y, 0 + handleMoveOffset, windowSize.y - handleMoveOffset);
        handle.anchoredPosition = newPos;

        GetColor();
    }

    private void GetColor()
    {
        RectTransform plane = ColorImageMask.GetComponent<RectTransform>();
        RectTransform handle = MainHandle.GetComponent<RectTransform>();
        Texture2D tex = (Texture2D)ColorImage.texture;
        
        int pixelX = tex.height;
        int pixelY = tex.width;
        pixelX = (int)(pixelX * ((float)handle.anchoredPosition.y / (float)plane.sizeDelta.y));
        pixelY = (int)(pixelY * ((float)handle.anchoredPosition.x / (float)plane.sizeDelta.x));

        Color col = tex.GetPixel(pixelY, pixelX);
        currentColor = MainHandle.color = col;
    }
}
