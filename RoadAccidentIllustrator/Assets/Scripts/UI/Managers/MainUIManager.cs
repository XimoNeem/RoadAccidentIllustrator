using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using RoadAccidentIllustrator.RAI_UI;
using RoadAccidentIllustrator.RAI_Events;
using RoadAccidentIllustrator.RAI_Settings;
using System.IO;
using UnityEngine.Networking;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager instance;
    TextData textData;

    [Header("Элементы тулбара")]
    public Transform itemsParent;
    public List<ToolbarItem> toolbarItems;
    public ToolbarItemType currentToolbarItem;
    [Space]
    [Header("Настройки списка")]
    public float scrollOffset;
    [Header("Текст для сериализации")]
    public TMP_Text NameText;
    public TMP_Text VersionText;
    public TMP_Text DescriptionText;
    public Button SiteLink;
    public Button VideoButton;
    public RawImage VideoPreview;
    public Button ShareVK;
    public Button ShareFB;
    public Button ShareINST;

    private void OnEnable()
    {
        EventBus.OnObjectInfo += SetObjectSettings;
    }

    private void OnDisable()
    {
        EventBus.OnObjectInfo -= SetObjectSettings;
    }
    void Start()
    {
        instance = this;
        textData = new TextData();
        StartCoroutine(LoadFromServer("https://pdd3d.com/textConfig.json"));

        SetToolbarItem(ToolbarItemType.MainPage);
    }

    private void SetObjectSettings()
    {
        SetToolbarItem(ToolbarItemType.ObjectSettings);
    }

    public void SetToolbarItem(ToolbarItemType type)
    {
        if(currentToolbarItem == ToolbarItemType.AddObject)
        {
            #if !UNITY_EDITOR
            if(ItemCreator.instance.isActiveAndEnabled)
            {
                ItemCreator.instance.SetCreationState(false);
            }
            #endif
        }
        
        currentToolbarItem = type;
        DeactivateAllItems();

        foreach (var item in GetToolbarItemsByType(type))
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }

        ActivateItems(type);

        itemsParent.position += new Vector3(0, scrollOffset, 0);

        EventBus.OnMenuItemSelected?.Invoke();

        if ( currentToolbarItem == ToolbarItemType.AddObject)
        {
            RAI_ListManager.instance.SetType(RoadAccidentIllustrator.RAI_ObjectSettings.RAI_ObjectType.Vehicle);
        }
    }

    private void ActivateItems(ToolbarItemType type)
    {
        if (type == ToolbarItemType.ObjectSettings)
        {
            ObjectSettingsManager.instance.SetSettingsManager();
        }
        else if(type == ToolbarItemType.MainSettings)
        {
            RAI_SettingsManager.instance.SetSettingsUI();
        }
    }

    private void DeactivateAllItems()
    {
        foreach (var toolItem in toolbarItems)
        {
            foreach (var item in toolItem.items)
            {
                if (item != null)
                {
                    item.SetActive(false);
                }
            }
        }
    }

    private List<GameObject> GetToolbarItemsByType(ToolbarItemType type)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (var item in toolbarItems)
        {
            if (item.type == type)
            {
                result = item.items;
                break;
            }
        }

        return result;
    }

    //private TextData LoadTextData()
    //{
    //    string path = "https://pdd3d.com/textConfig.json";
    //    StreamReader reader = new StreamReader(path);
    //    TextData result = JsonUtility.FromJson<TextData>(reader.ReadToEnd());
    //    reader.Close();
    //    return result;
    //}

    private void SetContentFromJson()
    {
        NameText.text = textData.Name;
        VersionText.text = textData.Version;
        DescriptionText.text = textData.Description;

        SiteLink.GetComponentInChildren<TMP_Text>().text = textData.SiteLink;
        if (textData.SiteLink != "")
        {
            SiteLink.onClick.AddListener(delegate { Application.OpenURL(textData.SiteLink); });
        }
        else
        {
            Destroy(SiteLink.transform.parent.gameObject);
        }

        Transform p = ShareVK.transform.parent;
        bool shareNoneNull = false;

        if (textData.VKComment == "")
        {
            Destroy(ShareVK.gameObject);
        }
        else
        {
            ShareVK.onClick.AddListener( delegate { Application.OpenURL(textData.VKComment); } );
            shareNoneNull = true;
        }

        if (textData.FBComment == "")
        {
            Destroy(ShareFB.gameObject);
        }
        else
        {
            ShareFB.onClick.AddListener(delegate { Application.OpenURL(textData.FBComment); });
            shareNoneNull = true;
        }

        if (textData.INSTComment == "")
        {
            Destroy(ShareINST.gameObject);
        }
        else
        {
            ShareINST.onClick.AddListener(delegate { Application.OpenURL(textData.INSTComment); });
            shareNoneNull = true;
        }

        if (!shareNoneNull) { Debug.Log(p.parent.name); Destroy(p.parent.gameObject); }
    }

    IEnumerator LoadFromServer(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            RAI_DebugManager.instance.ShowMessage("NetworkError", Color.red);
        }
        else
        {
            RAI_DebugManager.instance.ShowMessage(www.downloadHandler.text, Color.Lerp(Color.red, Color.yellow, 0.5f));
            textData = JsonUtility.FromJson<TextData>(www.downloadHandler.text);
            SetContentFromJson();
            StartCoroutine(LoadVideo(textData.VideoLink));
            //Debug.LogError(textData.ToString());
        }
    }

    IEnumerator LoadVideo(string url)
    {
        if (url == "")
        {
            Destroy(VideoButton.transform.parent.gameObject);
            yield return null;
        }

        else
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://i.ytimg.com/vi/" + url + "/hqdefault.jpg");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                RAI_DebugManager.instance.ShowMessage(request.error, Color.red);
            }
            else
            {
                //VideoPreview.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                VideoPreview.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                RAI_DebugManager.instance.ShowMessage(url, Color.green);
            }


            VideoButton.onClick.AddListener(delegate { Application.OpenURL("https://www.youtube.com/watch?v=" + url); });
        }
    }
}
