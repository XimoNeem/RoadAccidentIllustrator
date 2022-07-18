using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoadAccidentIllustrator.RAI_ObjectSettings;
using RoadAccidentIllustrator.RAI_Exceptions;

public class ObjectSettingsManager : MonoBehaviour
{
    MainUIManager mainManager;
    public static ObjectSettingsManager instance;

    public Movable currentItem;
    [Space]
    public List<SettingUIItem> settingUIItems;
    [Space]
    public GameObject NonSelectedWarning;
    public GameObject MultiSelectedWarning;
    public GameObject NonEditableWarning;
    public GameObject DeleteButton;
    [Space]
    public Transform parent;
    public List<GameObject> settingsObjects;
    [Header("Templates")]
    public GameObject LabelTemplate;
    public GameObject ToggleTemplate;
    public GameObject FloatTemplate;
    public GameObject EnumTemplate;
    public GameObject ColorTemplate;
    public GameObject roadSignTemplate;
    public GameObject TrafficLightTemplate;
    public GameObject TextureTemplate;

    private void OnEnable()
    {
        EventBus.OnObjectSelected += SetSettingsManager;
        EventBus.OnObjectDeselected += SetSettingsManager;
        EventBus.OnMenuItemSelected += CloseManager;
    }
    private void OnDisable()
    {
        EventBus.OnObjectSelected -= SetSettingsManager;
        EventBus.OnObjectDeselected -= SetSettingsManager;
        EventBus.OnMenuItemSelected -= CloseManager;
    }
    void Start()
    {
        instance = this;
        mainManager = FindObjectOfType<MainUIManager>();
    }

    public void SetSettingsManager()  //Включаем окно настроек
    {
        if (mainManager.currentToolbarItem == RoadAccidentIllustrator.RAI_UI.ToolbarItemType.ObjectSettings)
        {
            DeactivateAll();
            ObjectMover mover = FindObjectOfType<ObjectMover>();

            if (mover.selectedObjects.Count == 0)
            {
                NonSelectedWarning.SetActive(true);
            }
            else if (mover.selectedObjects.Count > 1)
            {
                MultiSelectedWarning.SetActive(true);
                //DeleteButton.SetActive(true);
            }

            else
            {
                Debug.Log(mover.selectedObjects[0].name);

                if (mover.selectedObjects[0].gameObject.GetComponent<Movable>())
                {
                    currentItem = mover.selectedObjects[0].GetComponent<Movable>();

                    if (currentItem.objectType == RAI_ObjectType.Decoration)
                    {
                        NonEditableWarning.SetActive(true);
                    }
                    else
                    {
                        SetSettingItem();
                    }
                }
                else
                {
                    NonEditableWarning.SetActive(true);
                }

                if(currentItem.allowedToDelete)
                {
                    DeleteButton.SetActive(true);
                    DeleteButton.transform.SetSiblingIndex(DeleteButton.transform.parent.childCount - 1);
                }
            }
        }
    }

    private void SetSettingItem()  //Создаем UI настроек конкретного объекта
    {
        //DeactivateAll();

        if (currentItem == null)
        {
            NonEditableWarning.SetActive(true);
            return;
        }

        if (currentItem.settingType == settingType.none)
        {
            NonEditableWarning.SetActive(true);
            return;
        }

        RAI_DebugManager.instance.ShowMessage(currentItem.name, Color.green);

        GameObject label = Instantiate(LabelTemplate, NonEditableWarning.transform.parent);
        parent = label.transform;
        settingsObjects.Add(label);

        label.GetComponentInChildren<UISettingItemHelper>().SetLabel(currentItem.objectName);

        List<SettingContainer> settings = currentItem.objectSettings;

        foreach (var item in settings)
        {
            settingsObjects.Add(SetItemFromTemplate(item));
        }

        if(currentItem.allowedToDelete)
        {
            DeleteButton.SetActive(true);
            DeleteButton.transform.SetSiblingIndex(DeleteButton.transform.parent.childCount - 1);
        }
    }

    private void DeactivateAll()
    {
        NonSelectedWarning.SetActive(false);
        MultiSelectedWarning.SetActive(false);
        NonEditableWarning.SetActive(false);
        DeleteButton.SetActive(false);

        foreach (var item in settingUIItems)
        {
            foreach (var uiItem in item.items)
            {
                if (uiItem != null)
                {
                    uiItem.SetActive(false);
                }
            }
        }

        foreach (var item in settingsObjects)
        {
            Destroy(item);
            
        }
        settingsObjects.Clear();
    }

    private void CloseManager()  //Удаляем UI настроек объекта
    {
        if (MainUIManager.instance.currentToolbarItem != RoadAccidentIllustrator.RAI_UI.ToolbarItemType.ObjectSettings)
        {
            DeactivateAll();
        }
    }

    #region TemplateStuff

    private GameObject SetItemFromTemplate(SettingContainer item)
    {
        GameObject newSettingObject = null;

        if (item is BoolContainer)
        {
            BoolContainer container = (BoolContainer)item;

            newSettingObject = Instantiate(ToggleTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);

            newSettingObject.GetComponent<Toggle>().isOn = container.value;
            newSettingObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate {SetCurrentItemSetting(container, newSettingObject.GetComponent<Toggle>().isOn); });
        }

        else if (item is FloatContainer)
        {
            FloatContainer container = (FloatContainer)item;

            newSettingObject = Instantiate(FloatTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);

            Slider sl = newSettingObject.GetComponentInChildren<Slider>();

            sl.maxValue = container.maxValue;
            sl.minValue = container.minValue;
            sl.value = container.value;

            sl.onValueChanged.AddListener( delegate { SetCurrentItemSetting(container, sl.value); } );
        }

        else if (item is TextureContainer)
        {
            TextureContainer container = (TextureContainer)item;

            newSettingObject = Instantiate(TextureTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);


            if (container.texture != null)
            {
                newSettingObject.GetComponentInChildren<RawImage>().texture = container.texture;
                newSettingObject.GetComponent<UISettingItemHelper>().itemName.text = container.texture.name;
            }
            else
            {
                newSettingObject.GetComponent<UISettingItemHelper>().itemName.text = "Пусто";
            }

            newSettingObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { FindObjectOfType<RoadPrintListController>().ShowList(); });
        }

        else if (item is ColorContainer)
        {
            ColorContainer container = (ColorContainer)item;

            newSettingObject = Instantiate(ColorTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);

            newSettingObject.GetComponent<ColorSettingHelper>().SetColor(container.value);
            newSettingObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { RAI_ColorPickerManager.instance.StartManager(SetCurrentItemSetting, container); });
        }

        else if (item is RoadSignContainer)
        {
            RoadSignContainer container = (RoadSignContainer)item;

            newSettingObject = Instantiate(roadSignTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);

            if (container.texture != null)
            {
                newSettingObject.GetComponentInChildren<RawImage>().texture = container.texture;
                newSettingObject.GetComponent<UISettingItemHelper>().itemName.text = container.texture.name;
            }
            else
            {
                newSettingObject.GetComponent<UISettingItemHelper>().itemName.text = "Пусто";
            }



            newSettingObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { FindObjectOfType<TrafficSignListController>().ShowList(); });
        }

        else if (item is TrafficLightContainer)
        {
            TrafficLightContainer container = (TrafficLightContainer)item;

            newSettingObject = Instantiate(TrafficLightTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);

            foreach (var groupItem in newSettingObject.GetComponent<TrafficLightSettingsController>().groupHelpers)
            {
                groupItem.gameObject.SetActive(false);
            }

            TrafficLightGroupHelper currentHelper = null;

            foreach (var groupItem in newSettingObject.GetComponent<TrafficLightSettingsController>().groupHelpers)
            {
                if (groupItem.groupType == container.trafficLightType)
                {
                    groupItem.Activate();
                    currentHelper = groupItem;
                }
                else
                {
                    Destroy(groupItem.gameObject);
                }
            }

            if (currentHelper == null)
            {
                RAI_DebugManager.instance.ShowMessage("TrafficLightGroupHelper ERROR", Color.red);
            }
            else
            {
                foreach (var uiItem in currentHelper.GetComponentsInChildren<TrafficLightItemHelper>())
                {
                    foreach (var s in container.textures)
                    {
                        if (uiItem.lightPosition == s.lightPosition)
                        {
                            uiItem.signalType = s.lightType;
                            uiItem.signalValue = (int)s.lightType;
                            uiItem.GetComponentInChildren<RawImage>().texture = RAI_TrafficLightSignManager.instance.GetSignalTexture(s.lightType);
                        }
                    }
                }
            }



            //newSettingObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { FindObjectOfType<TrafficSignListController>().ShowList(); });
            //newSettingObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { currentItem.ApplySettings(); });
        }

        else if (item is EnumContainer)
        
        {
            EnumContainer container = (EnumContainer)item;

            newSettingObject = Instantiate(EnumTemplate, parent);
            newSettingObject.GetComponent<UISettingItemHelper>().SetLabel(item.name);

            List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();

            foreach (var nameItem in container.names)
            {
                data.Add(new TMP_Dropdown.OptionData(nameItem));
            }

            TMP_Dropdown dd = newSettingObject.GetComponent<TMP_Dropdown>();

            dd.ClearOptions();
            dd.AddOptions(data);
            dd.value = container.value;

            dd.onValueChanged.AddListener( delegate { SetCurrentItemSetting(container, dd.value); } );
        }

        return newSettingObject;
    }

    #endregion TemplateStuff

    #region SetSettings

    public void SetCurrentItemSetting(SettingContainer currentSetting, bool newValue)
    {
        if (SettingsManager.GetSettingType(currentSetting) == containerType.BoolContainer)
        {
            BoolContainer container = (BoolContainer)currentSetting;
            container.value = newValue;

            currentItem.ApplySettings();
        }
        else
        {
            throw new RAI_Exception.NoSettingException("WrongContainerType");
        }
    }

    public void SetCurrentItemSetting(SettingContainer currentSetting, Color newValue)
    {
        if (SettingsManager.GetSettingType(currentSetting) == containerType.ColorContainer)
        {
            ColorContainer container = (ColorContainer)currentSetting;
            container.value = newValue;

            currentItem.ApplySettings();
        }
        else
        {
            throw new RAI_Exception.NoSettingException("WrongContainerType");
        }
    }

    public void SetCurrentItemSetting(SettingContainer currentSetting, float newValue)
    {
        if (SettingsManager.GetSettingType(currentSetting) == containerType.FloatContainer)
        {
            FloatContainer container = (FloatContainer)currentSetting;
            container.value = newValue;

            currentItem.ApplySettings();
        }
        else
        {
            throw new RAI_Exception.NoSettingException("WrongContainerType");
        }
    }

    public void SetCurrentItemSetting(SettingContainer currentSetting, Texture2D newValue)
    {
        if (SettingsManager.GetSettingType(currentSetting) == containerType.FloatContainer)
        {
            TextureContainer container = (TextureContainer)currentSetting;
            container.texture = newValue;

            currentItem.ApplySettings();
        }
        else
        {
            throw new RAI_Exception.NoSettingException("WrongContainerType");
        }
    }

    public void SetCurrentItemSetting(SettingContainer currentSetting, int newValue)
    {
        if (SettingsManager.GetSettingType(currentSetting) == containerType.EnumContainer)
        {
            EnumContainer container = (EnumContainer)currentSetting;
            container.value = newValue;

            currentItem.ApplySettings();
        }
        else
        {
            throw new RAI_Exception.NoSettingException("WrongContainerType");
        }
    }

    #endregion SetSettings
}
