using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;
using UnityEngine.Events;

public class Movable : MonoBehaviour
{
    public bool selected = false;
    public bool editable = false;
    public bool allowedToMove = true;
    public bool allowedToRotate = true;
    public bool allowedToEdit = true;
    public bool allowedToDelete = true;
    [Space]
    public Texture previewImage;
    [Range(-10, 10)]
    public float previewZoom;
    [Range(-10, 10)]
    public float previewVerticalOffset;
    [Header("Имя объекта")]
    public string objectName;
    public RAI_ObjectType objectType;
    [Header("Настройки")]
    public settingType settingType;
    public List<SettingContainer> objectSettings;
    [Header("Значения по умолчанию")]
    public List<DefaultValue> defaultValues;

    public virtual void Start()
    {
        //allowedToDelete = true;
        allowedToEdit = true;

        objectSettings = SettingsManager.GetSettingsByType(settingType);

        foreach (var item in defaultValues)
        {
            if (SettingsManager.HasSetting(item.type, this))
            {
                containerType type = SettingsManager.GetSettingType(SettingsManager.GetSettingContainer(this, item.type));

                if (type == containerType.BoolContainer)
                {
                    SettingsManager.SetItemSetting(item.type, item.BoolValue, this);
                }
                else if (type == containerType.EnumContainer)
                {
                    SettingsManager.SetItemSetting(item.type, item.IntValue, this);
                }
            }
            else
            {

            }
        }
    }

    private void OnMouseDown()
    {
        OnSelected();
    }

    private void OnMouseUp()
    {
        OnDeselected();
    }

    public virtual void ApplySettings()
    {
        RAI_DebugManager.instance.ShowMessage("Applying settings", Color.green);
    }

    public virtual void DeleteMovable()
    {
        Destroy(this.gameObject);
    }

    public virtual void OnSelected()
    {

    }

    public virtual void OnDeselected()
    {

    }

    public virtual void OnRotated()
    {

    }

    public virtual void OnMoved()
    {
        FixPosition();
    }

    public virtual void OnRotatedEnd()
    {

    }

    public virtual void OnMovedEnd()
    {
        FixPosition();
    }

    private void FixPosition()
    {
        float verticalPositionDistanceRay = 500;
        Ray verticalPositionRay = new Ray();
        int layerMaskOnlyCI = LayerMask.NameToLayer("CameraInput");
        int layerMaskOnlyGizmo = LayerMask.NameToLayer("Gizmo");
        int layerMaskWithoutGizmo = ~((1 << layerMaskOnlyCI) | (1 << layerMaskOnlyGizmo));

        RaycastHit hit;

        foreach (var item in this.gameObject.GetComponents<Collider>())
        {
            item.enabled = false;
        }

        verticalPositionRay.origin = this.transform.position + new Vector3(0, verticalPositionDistanceRay, 0);
        verticalPositionRay.direction = Vector3.down;

        Debug.DrawRay(verticalPositionRay.origin, verticalPositionRay.direction, Color.red);

        Physics.Raycast(verticalPositionRay, out hit, Mathf.Infinity, layerMaskWithoutGizmo);

        this.transform.position = hit.point;

        foreach (var item in this.gameObject.GetComponents<Collider>())
        {
            item.enabled = true;
        }
        
    }
}