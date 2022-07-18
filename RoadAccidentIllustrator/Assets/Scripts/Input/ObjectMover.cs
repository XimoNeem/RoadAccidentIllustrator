using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_Events;

public class ObjectMover : MonoBehaviour
{
    public static ObjectMover instance;
    public bool editable;
    public List<Transform> selectedObjects;
    [Space]
    [Header("Hot keys")]
    public KeyCode AddKey = KeyCode.LeftShift;
    public KeyCode RemoveKey = KeyCode.LeftControl;
    public KeyCode CopyKey = KeyCode.C;
    public KeyCode PasteKey = KeyCode.V;
    [Space]
    public LayerMask selectionMask;
    public LayerMask objectControl;
    [Space]
    public GameObject selectedMarker;
    public GameObject Gizmo;

    private Transform currentGizmo;

    private void Start()
    {
        instance = this;
    }

    public void SetEditable(bool value)
    {
        editable = value;
    }

    private void OnEnable()
    {
        EventBus.OnObjectMove += MoveObject;
        EventBus.OnObjectRotate += RotateObject;
    }

    private void OnDisable()
    {
        EventBus.OnObjectMove -= MoveObject;
        EventBus.OnObjectRotate -= RotateObject;
    }

    void MoveObject()
    {
        Vector3 posOffset = currentGizmo.position - selectedObjects[0].position;

        foreach (var item in selectedObjects)
        {
            item.position += posOffset;           
        }
    }

    void RotateObject()
    {
        Quaternion newRotation = currentGizmo.rotation;

        foreach (var item in selectedObjects)
        {
            item.rotation = newRotation;
        }
    }

    public void DeleteObject()
    {
        foreach (var item in selectedObjects)
        {
            if (item.GetComponent<Movable>())
            {
                item.GetComponent<Movable>().DeleteMovable();
            }
        }

        ObjectMover.instance.ClearTargets();
        FindObjectOfType<ObjectSettingsManager>().SetSettingsManager();
    }

    public void AddTarget(Transform target)
    {
        if (selectedObjects.Contains(target)) return;

        AddSelectedMarker(target);
        selectedObjects.Add(target);

        SetGizmo();
    }

    void RemoveTarget(Transform target)
    {
        if (!selectedObjects.Contains(target)) return;

        RemoveSelectedMarker(target);
        selectedObjects.Remove(target);

        SetGizmo();
    }

    public void ClearTargets()
    {
        RemoveAllMarkers();

        selectedObjects.Clear();

        SetGizmo();
    }

    void ClearAndAddTarget(Transform target)
    {
        ClearTargets();

        AddTarget(target);

        //SetGizmo();
    }

    private void LateUpdate()
    {
        GetTarget();

        UpdateSelected();
    }

    public bool isEditing()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, objectControl))
        {
            if (hitInfo.transform.tag == "gizmo")
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateSelected()
    {
        foreach (var item in FindObjectsOfType<Movable>())
        {
            item.selected = false;
        }

        foreach (var item in selectedObjects)
        {
            if (item != null && item.GetComponent<Movable>())
            {
                item.GetComponent<Movable>().selected = true;

            }
        }
    }

    void GetTarget()
    {
        if (editable)
        {
            if (Input.GetMouseButtonDown(0) & !isEditing())
            {
                bool isAdding = Input.GetKey(AddKey);
                bool isRemoving = Input.GetKey(RemoveKey);

                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, selectionMask))
                {
                    if (hitInfo.transform.tag != "gizmo")
                    {


                        Transform target = hitInfo.transform;

                        if (isAdding)
                        {
                            AddTarget(target);
                        }
                        else if (isRemoving)
                        {
                            RemoveTarget(target);
                        }
                        else if (!isAdding && !isRemoving)
                        {
                            ClearAndAddTarget(target);
                        }

                        EventBus.OnObjectSelected?.Invoke();
                    }
                }
                else
                {
                    if (!isAdding && !isRemoving)
                    {


                        ClearTargets();

                        EventBus.OnObjectDeselected?.Invoke();
                    }
                }
            }
        }
    }

    void SetGizmo()
    {
        if (selectedObjects.Count > 0)
        {
            if (currentGizmo == null)
            {
                currentGizmo = Instantiate(Gizmo).transform;
                currentGizmo.position = selectedObjects[0].position;
                currentGizmo.rotation = selectedObjects[0].rotation;
            }
            else
            {
                currentGizmo.position = selectedObjects[0].position;
            }

            if (selectedObjects.Count == 1)
            {
                if (selectedObjects[0].GetComponent<Movable>())
                {
                    Movable item = selectedObjects[0].GetComponent<Movable>();
                    currentGizmo.GetComponent<GizmoParentHelper>().InfoTool.SetActive(item.allowedToEdit);
                    currentGizmo.GetComponent<GizmoParentHelper>().MoveTool.SetActive(item.allowedToMove);
                    currentGizmo.GetComponent<GizmoParentHelper>().RotateTool.SetActive(item.allowedToRotate);
                }
                else
                {
                    currentGizmo.GetComponent<GizmoParentHelper>().InfoTool.SetActive(false);
                }
            }
            else if (selectedObjects.Count > 1)
            {
                currentGizmo.GetComponent<GizmoParentHelper>().InfoTool.SetActive(false);
            }
        }
        else
        {
            if (FindObjectOfType<GizmoParentHelper>())
            {
                Destroy(FindObjectOfType<GizmoParentHelper>().gameObject);
                currentGizmo = null;
            }
        }
    }

    void AddSelectedMarker(Transform target)
    {
        GameObject.Instantiate(selectedMarker, target);
    }

    void RemoveSelectedMarker(Transform target)
    {
        Destroy(target.GetComponentInChildren<SelectedMarker>().gameObject);
    }

    void RemoveAllMarkers()
    {
        foreach (var item in FindObjectsOfType<SelectedMarker>())
        {
            Destroy(item.gameObject);
        }
    }

}
