using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoadAccidentIllustrator.RAI_ObjectSettings;
using System;

public class RAI_ListManager : MonoBehaviour
{
    public static RAI_ListManager instance;
    public List<Movable> allItems;
    public List<Movable> currentItems;

    public List<ItemUIHelper> currentButtons;

    public List<ItemUIHelper> requestPreview;
    [Space]
    public GameObject itemUITemplate;
    public Transform itemUIParent;
    [Space]
    public RAI_ObjectType currentType;
    public Enum currentSubtype;
    public int subtypeValue;
    [Space]
    public TMP_Dropdown subtypeDropdown;

    private void OnEnable()
    {
        EventBus.OnMenuItemSelected += ClearButtons;
    }

    private void OnDisable()
    {
        EventBus.OnMenuItemSelected -= ClearButtons;
    }
    void Start()
    {
        instance = this;
        SetType(RAI_ObjectType.Road);

        Movable[] loadedItems = Resources.LoadAll<Movable>("Items");

        foreach (var item in loadedItems)
        {
            allItems.Add(item);
        }
    }

    public void SetType(RAI_ObjectType type)
    {
        currentType = type;
        subtypeDropdown.ClearOptions();
        subtypeValue = 0;

        switch (type)
        {
            case RAI_ObjectType.Road:
                {
                    subtypeDropdown.AddOptions(GetOptions(new RoadType()));
                    break;
                }
            case RAI_ObjectType.Vehicle:
                {
                    subtypeDropdown.AddOptions(GetOptions(new VehicleType()));
                    break;
                }
            case RAI_ObjectType.TrafficSign:
                {
                    subtypeDropdown.AddOptions(GetOptions(new TrafficSignType()));
                    break;
                }
            case RAI_ObjectType.TrafficLight:
                {
                    subtypeDropdown.AddOptions(GetOptions(new TrafficLightType()));
                    break;
                }
            case RAI_ObjectType.RoadMark:
                {
                    subtypeDropdown.AddOptions(GetOptions(new RoadMarkType()));
                    break;
                }
            case RAI_ObjectType.Decoration:
                {
                    subtypeDropdown.AddOptions(GetOptions(new DecorationType()));
                    break;
                }
            default: break;
        }

        FilterItems();
    }

    public void SetSubtype(int value)
    {
        subtypeValue = value;
        FilterItems();
    }

    private void FilterItems()
    {
        currentItems.Clear();

        foreach (var movable in allItems)
        {
            switch (currentType)
            {
                case RAI_ObjectType.Road:
                    {
                        if (movable is PDDERoad)
                        {
                            PDDERoad item = (PDDERoad)movable;
                            if ((int)item.subtype == subtypeValue)
                            {
                                currentItems.Add(movable);
                            }
                        }
                        break;
                    }
                case RAI_ObjectType.Vehicle:
                    {
                        if (movable is PDDEVehicle)
                        {
                            PDDEVehicle item = (PDDEVehicle)movable;
                            if ((int)item.subtype == subtypeValue)
                            {
                                currentItems.Add(movable);
                            }
                        }
                        break;
                    }
                case RAI_ObjectType.TrafficSign:
                    {
                        if (movable is PDDETrafficSign)
                        {
                            PDDETrafficSign item = (PDDETrafficSign)movable;
                            if ((int)item.subtype == subtypeValue)
                            {
                                currentItems.Add(movable);
                            }
                        }
                        break;
                    }
                case RAI_ObjectType.TrafficLight:
                    {
                        if (movable is PDDETrafficLight)
                        {
                            PDDETrafficLight item = (PDDETrafficLight)movable;
                            if ((int)item.subtype == subtypeValue)
                            {
                                currentItems.Add(movable);
                            }
                        }
                        break;
                    }
                case RAI_ObjectType.RoadMark:
                    {
                        if (movable is PDDERoadMark)
                        {
                            PDDERoadMark item = (PDDERoadMark)movable;
                            if ((int)item.subtype == subtypeValue)
                            {
                                currentItems.Add(movable);
                            }
                        }
                        break;
                    }
                case RAI_ObjectType.Decoration:
                    {
                        if (movable is PDDEDecoration)
                        {
                            PDDEDecoration item = (PDDEDecoration)movable;
                            if ((int)item.subtype == subtypeValue)
                            {
                                currentItems.Add(movable);
                            }
                        }
                        break;
                    }
                default: break;
            }
        }

        AddItemsToList(currentItems);
    }

    private void AddItemsToList(List<Movable> movables)
    {
        ClearButtons();
        
        foreach (var movable in movables)
        {
            ItemUIHelper item = Instantiate(itemUITemplate, itemUIParent).GetComponent<ItemUIHelper>();
            item.CreateItem(movable);

            currentButtons.Add(item);

            if (movable.previewImage != null)
            {
                item.preview.texture = movable.previewImage;
            }
            else
            {
                requestPreview.Add(item);
            }
        }

        StartCoroutine(SetPreviews());
    }

    private List<TMP_Dropdown.OptionData> GetOptions(Enum currentType)
    {
        currentSubtype = currentType;

        List<TMP_Dropdown.OptionData> result = new List<TMP_Dropdown.OptionData>();

        foreach (var item in Enum.GetNames(currentType.GetType()))
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.Replace("_", " ");

            result.Add(option);
        }

        return result;
    }

    IEnumerator SetPreviews()
    {
        foreach (var item in requestPreview)
        {
            Texture texture = new Texture2D(512, 512);
            PreviewCreator.instance.CreateTexture(item.original, out texture);

            item.preview.texture = texture;
            item.preview.color = Color.white;
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        }

        requestPreview.Clear();
    }

    private void ClearButtons()
    {
        foreach (ItemUIHelper item in currentButtons)
        {
            Destroy(item.gameObject);
        }
        currentButtons.Clear();
    }
}
