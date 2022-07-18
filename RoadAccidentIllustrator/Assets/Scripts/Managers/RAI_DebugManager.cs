using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAI_DebugManager : MonoBehaviour
{
    public bool debuging = true;
    public int maxMessageAmount = 100;
    public static RAI_DebugManager instance;
    public Transform parent;
    public GameObject messageItem;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetDebugState(true);
    }

    public void ShowMessage(string message)
    {
        if (!debuging)
            return;

        GameObject item = Instantiate(messageItem, parent);
        item.GetComponentInChildren<TMPro.TMP_Text>().text = message;

        if (parent.childCount > maxMessageAmount)
        {
            int length = parent.childCount - maxMessageAmount;

            for (int i = 0; i < length; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        parent.transform.position += new Vector3(0, 50, 0);
    }

    public void ShowMessage(string message, Color textColor)
    {
        if (!debuging)
            return;

        GameObject item = Instantiate(messageItem, parent);
        item.GetComponentInChildren<TMPro.TMP_Text>().text = message;
        item.GetComponentInChildren<TMPro.TMP_Text>().color = textColor;
        

        if (parent.childCount > maxMessageAmount)
        {
            int length = parent.childCount - maxMessageAmount;

            for (int i = 0; i < length; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        parent.transform.position += new Vector3(0, 50, 0);
    }

    public void SetDebugState(bool state)
    {
        debuging = state;
        parent.gameObject.SetActive(state);
    }

    public void ClearDebug()
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
