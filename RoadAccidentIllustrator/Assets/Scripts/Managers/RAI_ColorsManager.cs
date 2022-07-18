using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RAI_ColorsManager : MonoBehaviour
{
    public static RAI_ColorsManager instance;
    [Header("Normal Button")]
    public ColorBlock normalButton;
    [Space]
    [Header("Cancel Button")]
    public ColorBlock cancelButton;

    private void Awake()
    {
        instance = this;
    }
    
}
