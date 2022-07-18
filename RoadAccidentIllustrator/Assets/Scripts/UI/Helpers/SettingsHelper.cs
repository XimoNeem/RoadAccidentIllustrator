using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHelper : MonoBehaviour
{
    ObjectSettingsManager manager;

    private void Start()
    {
        manager = FindObjectOfType<ObjectSettingsManager>();


    }
    public void UISetLight(bool state)
    {
        manager.currentItem.GetComponent<PDDEVehicle>().ForwardLight.SetActive(state);
    }

    public void UISetLightFar(bool state)
    {
        manager.currentItem.GetComponent<PDDEVehicle>().FarForwardLight.SetActive(state);
    }

    public void UISetTurnLightLeft(bool state)
    {
        manager.currentItem.GetComponent<PDDEVehicle>().LeftTurn.SetActive(state);
    }

    public void UISetTurnLightRight(bool state)
    {
        manager.currentItem.GetComponent<PDDEVehicle>().RightTurn.SetActive(state);
    }

    public void UISetStopLight(bool state)
    {
        manager.currentItem.GetComponent<PDDEVehicle>().StopLight.SetActive(state);
    }

    public void UISetCrossroadOption(int value)
    {
        foreach (var item in manager.currentItem.GetComponent<PDDERoad>().options)
        {
            item.SetActive(false);
        }

        manager.currentItem.GetComponent<PDDERoad>().options[value].SetActive(true);
    }
}
