using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Runtime.InteropServices;

public class RAI_RenderManager : MonoBehaviour
{
	[Header("UI")]
	public TMP_InputField heightInput;
	public TMP_InputField widthInput;
	public TMP_InputField nameInput;
	public TMP_Dropdown formatInput;
	[Space]
	public Texture2D finalTexture;
	[Space]
    public Camera renderCamera;
    public Camera mainCamera;
    public ObjectMover mover;
    public RenderTexture targetTex;
    public Texture2D watermarkTemplate;
    public bool AddWatermark;
	[Range (0, 1)]
	public float WatermarkStrength;
    [Space]
    public List<GameObject> UIItems;
	public RawImage previewImage;
	public float sizeMultiplayer = 0.9f;

    private void Start()
    {
		mover = FindObjectOfType<ObjectMover>();

    }

	[DllImport("__Internal")]
	public static extern void DownloadFile(byte[] array, int byteLength, string fileName);

	public void Render()
    {
        ActivateItems(true);;

        renderCamera = FindObjectOfType<RAI_RenderSettingsManager>().currentRenderCamera;
		
		QualitySettings.SetQualityLevel(5);

		RenderTexture newTex = new RenderTexture(int.Parse(widthInput.text), int.Parse(heightInput.text), 16, RenderTextureFormat.ARGB32);
		newTex.Create();

		renderCamera.targetTexture = newTex;
		renderCamera.Render();


		finalTexture = toTexture2D(newTex);
		if (AddWatermark)
        {
			finalTexture = setWatermark(finalTexture, watermarkTemplate);
        }

		previewImage.texture = finalTexture;
		previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(finalTexture.width, finalTexture.height);
		renderCamera.targetTexture = targetTex;
	}

    public void ActivateItems(bool state)
    {
        foreach (var item in UIItems)
        {
            item.SetActive(state);
        }

        mover.SetEditable(!state);
        mainCamera.GetComponent<FreeFlyCamera>().enabled = !state;

    }

    public void ExitRenderPage()
    {
        ActivateItems(false);
		QualitySettings.SetQualityLevel(0);
    }

	public void SaveImage()
    {
		byte[] imageBytes;

		if (formatInput.captionText.text == "png")
		{
			imageBytes = finalTexture.EncodeToPNG();
		}
		else
		{
			imageBytes = finalTexture.EncodeToJPG();
		}

		int byteValue = imageBytes.Length;
		string fileName = nameInput.text + "." + formatInput.captionText.text;

		Debug.Log(fileName);

		DownloadFile(imageBytes, byteValue, fileName);

		ActivateItems(false);
	}

	public void SetPreviewSize(float value)
	{
		sizeMultiplayer = value;
		Vector3 previewSize = Vector3.one * sizeMultiplayer;
		previewImage.transform.localScale = previewSize;
	}
	public Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(int.Parse(widthInput.text), int.Parse(heightInput.text), TextureFormat.RGB24, false);
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		return tex;
	}

	Texture2D setWatermark(Texture2D background, Texture2D add)
	{
		int startX = 0;
		int startY = 0;
		int endY = background.height;
		int endX = background.width;

		for (int x = startX; x < endX; x++)
		{

			for (int y = startY; y < endY; y++)
			{
				Color bgColor = background.GetPixel(x, y);
				Color wmColor = add.GetPixel(x - startX, y - startY);

				Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a * WatermarkStrength);

				background.SetPixel(x, y, final_color);
			}
		}

		background.Apply();
		return background;
	}
}
