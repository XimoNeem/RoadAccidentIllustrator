using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_Settings;
using System.Reflection;

public class RAI_SettingsManager : MonoBehaviour
{
    public static RAI_SettingsManager instance;
    #region Items

    public Camera editorCamera;
    public Camera editor2DCamera;
    public FreeFlyCamera flyCamera;

    #endregion Items
    public RAI_MainSettingsContainer currentSettings;
    public List<MainSettingItemHelper> helpers;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentSettings = new RAI_MainSettingsContainer();
        //SetSettingsUI();
    }

    private void ApplySettings()
    {
        flyCamera.SetMoveSpeed(currentSettings.cameraSpeed);
        editorCamera.fieldOfView = currentSettings.cameraViewSize;

        if(currentSettings.cameraUpsideDown)
        {
            editorCamera.gameObject.SetActive(false);
            editor2DCamera.gameObject.SetActive(true);

            FindSettingByType(SettingItemType.cameraMoveSpeed).gameObject.GetComponent<UnityEngine.UI.Slider>().interactable = false;
            FindSettingByType(SettingItemType.cameraViewSize).gameObject.GetComponent<UnityEngine.UI.Slider>().interactable = false;
        }
        else
        {
            editorCamera.gameObject.SetActive(true);
            editor2DCamera.gameObject.SetActive(false);

            FindSettingByType(SettingItemType.cameraMoveSpeed).gameObject.GetComponent<UnityEngine.UI.Slider>().interactable = true;
            FindSettingByType(SettingItemType.cameraViewSize).gameObject.GetComponent<UnityEngine.UI.Slider>().interactable = true;
        }
    }

    public void SetSettingsUI()
    {
        FindSettingByType(SettingItemType.cameraMoveSpeed).SetValue(currentSettings.cameraSpeed);
        FindSettingByType(SettingItemType.cameraUpsideDown).SetValue(currentSettings.cameraUpsideDown);
        FindSettingByType(SettingItemType.cameraViewSize).SetValue(currentSettings.cameraViewSize);

        FindSettingByType(SettingItemType.vegetationRandom).SetValue(currentSettings.vegetationRandom);
        FindSettingByType(SettingItemType.vegetationNonstopCreating).SetValue(currentSettings.vegetationNonstopCreating);

        /*
        FieldInfo[] items = currentSettings.GetType().GetFields();
        Debug.Log(items.Length);
        foreach (var item in items)
        {
            /*
            MainSettingItemHelper helper = FindSettingByType(item.type);
            if (helper != null)
            {
                helper.SetValue(item.container.GetValue().value);
            }
        Debug.Log(item.Nam);
        }
        */
    }

    public void UpadateSettings()
    {

        currentSettings.cameraSpeed = FindSettingByType(SettingItemType.cameraMoveSpeed).GetValue();
        currentSettings.cameraUpsideDown = FindSettingByType(SettingItemType.cameraUpsideDown).GetState();
        currentSettings.cameraViewSize = FindSettingByType(SettingItemType.cameraViewSize).GetValue();

        currentSettings.vegetationNonstopCreating = FindSettingByType(SettingItemType.vegetationNonstopCreating).GetState();
        currentSettings.vegetationRandom = FindSettingByType(SettingItemType.vegetationRandom).GetState();

        ApplySettings();
    }

    private MainSettingItemHelper FindSettingByType(SettingItemType type)
    {
        foreach (var item in helpers)
        {
            if (item.settingType == type)
            {
                return item;
            }
        }
        
        return null;
    }
}
