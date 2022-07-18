using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour
{
    public GameObject item;
    public Transform parent;

    public void Create()
    {
        Instantiate(item, parent);
    }

    public void Destroy()
    {
        Destroy(item);
    }
}
