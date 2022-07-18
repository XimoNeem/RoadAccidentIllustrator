using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoadAccidentIllustrator.RAI_Events;
using TMPro;

public class ItemCreator : MonoBehaviour
{
    public static ItemCreator instance;

    public bool getInput;
    public bool creationAllowed;
    public GameObject currentOriginal;
    public LayerMask raycastLayer;
    public GameObject currentInstance;

    RaycastHit hitInfo;
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        instance = this;
    }

    private void Update()
    {
        if (creationAllowed & getInput)
        {
            CalculatePosition();

            if (Input.GetMouseButtonDown(0))
            {
                CreateItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCreationState(false);

            
        }
    }

    public void SetCreationState(bool state)
    {
        if (state == false)
        {
            Destroy(currentInstance);
            ObjectMover.instance.editable = true;

            foreach (var item in FindObjectsOfType<ItemUIHelper>())
            {
                if (item != this)
                {
                    item.GetComponentInChildren<Button>().interactable = true;
                    item.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = "Добавить";
                    item.GetComponentInChildren<Button>().Select();
                    item.GetComponentInChildren<Button>().colors = RAI_ColorsManager.instance.normalButton;
                    item.isAdding = false;
                }
            }
        }
        else
        {
            ObjectMover.instance.editable = false;
        }

        creationAllowed = state;
    }

    public void SetInputState(bool state)
    {
        getInput = state;
    }

    public void SetOriginal(GameObject original)
    {
        creationAllowed = true;
        currentOriginal = original;

        if (original.GetComponent<Movable>())
        {
            original.GetComponent<Movable>().editable = false;
        }
    }

    void CalculatePosition()
    {
        if (currentInstance == null)
        {
            currentInstance = Instantiate(currentOriginal);
            currentInstance.GetComponent<Movable>().enabled = false;
        }

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, raycastLayer))
        {
            currentInstance.transform.position = hitInfo.point;
        }

        float verticalPositionDistanceRay = 500;
        Ray verticalPositionRay = new Ray();

        RaycastHit hit;

        foreach (var item in currentInstance.gameObject.GetComponents<Collider>())
        {
            item.enabled = false;
        }

        verticalPositionRay.origin = currentInstance.transform.position + new Vector3(0, verticalPositionDistanceRay, 0);
        verticalPositionRay.direction = Vector3.down;

        Debug.DrawRay(verticalPositionRay.origin, verticalPositionRay.direction, Color.red);

        Physics.Raycast(verticalPositionRay, out hit);

        currentInstance.transform.position = hit.point;

        foreach (var item in currentInstance.gameObject.GetComponents<Collider>())
        {
            item.enabled = true;
        }
    }

    void CreateItem()
    {
        currentInstance.GetComponent<Movable>().enabled = true;
        currentInstance.GetComponent<Movable>().editable = true;
        currentInstance = Instantiate(currentOriginal);
        currentInstance.GetComponent<Movable>().enabled = false;

        if (!currentOriginal.GetComponent<PDDERoadPoint>() && !currentOriginal.GetComponent<PDDERoadMarkPoint>())
        {
            SetCreationState(false);
            /*
            if (!currentOriginal.GetComponent<RAI_Decoration>())
            {
                SetCreationState(false);
            }
            else if (currentOriginal.GetComponent<RAI_Decoration>().subtype == RoadAccidentIllustrator.RAI_ObjectSettings.DecorationType.Растительность)
            {
                if (RAI_SettingsManager.instance.currentSettings.vegetationNonstopCreating == false)
                {
                    SetCreationState(false);
                }
            }
            */
        }

        currentInstance.GetComponent<Movable>().OnMovedEnd();

        EventBus.OnObjectCreated?.Invoke();
    }
}
