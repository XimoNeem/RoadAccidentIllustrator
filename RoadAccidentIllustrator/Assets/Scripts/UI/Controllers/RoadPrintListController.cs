using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RoadPrintListController : MonoBehaviour
{
    public FreeFlyCamera cam;
    public ObjectMover mover;
    public GameObject ListObject;
    [Range(0, 1)]
    public float previewTrans;
    [Space]
    public TMP_Dropdown typeDropdown;
    public int type = 0;
    public Transform parent;
    public GameObject itemTemplate;
    [Space]
    public List<Texture2D> group_1;
    public List<Texture2D> group_2;
    public List<Texture2D> group_3;
    public List<Texture2D> group_4;
    public List<Texture2D> group_5;
    public List<Texture2D> group_6;
    public List<Texture2D> group_7;
    public List<Texture2D> group_8;
    public List<Texture2D> group_9;


    public void ShowList()
    {
        cam.enabled = false;
        mover.SetEditable(false);

        ListObject.SetActive(true);
        SetNewType();
    }

    public void HideList()
    {
        ClearOptions();
        //ListObject.GetComponent<Animator>().SetTrigger("Close");
        ListObject.SetActive(false);
        //ListObject.GetComponent<Image>().raycastTarget = false;

        cam.enabled = true;
        mover.SetEditable(true);
    }

    IEnumerator CloseList()
    {
        yield return new WaitForEndOfFrame();
        ListObject.SetActive(false);
    }

    public void SetType(int value)
    {
        type = value;
        SetNewType();
    }

    private void ClearOptions()
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void SetNewType()
    {
        ClearOptions();

        switch (type)
        {
            case 1 - 1:
                {
                    SetTypeList(group_1);
                    break;
                }
            case 2 - 1:
                {
                    SetTypeList(group_2);
                    break;
                }
            case 3 - 1:
                {
                    SetTypeList(group_3);
                    break;
                }
            case 4 - 1:
                {
                    SetTypeList(group_4);
                    break;
                }
            case 5 - 1:
                {
                    SetTypeList(group_5);
                    break;
                }
            case 6 - 1:
                {
                    SetTypeList(group_6);
                    break;
                }
            case 7 - 1:
                {
                    SetTypeList(group_7);
                    break;
                }
            case 8 - 1:
                {
                    SetTypeList(group_8);
                    break;
                }
            case 9 - 1:
                {
                    SetTypeList(group_9);
                    break;
                }
            default: break;
        }
    }

    private void SetTypeList(List<Texture2D> textures)
    {
        foreach (var item in textures)
        {
            GameObject temp = Instantiate(itemTemplate, parent);
            temp.GetComponentInChildren<TMP_Text>().text = item.name;
            temp.GetComponentInChildren<RawImage>().texture = item;
            Color newColor = temp.GetComponentInChildren<RawImage>().color;
            newColor.a = previewTrans;
            temp.GetComponentInChildren<RawImage>().color = newColor;
            temp.GetComponent<RoadPrintUIHelper>().currentTex = item;

            temp.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            //temp.GetComponentInChildren<Button>().onClick.AddListener( delegate { link(container, (Texture2D)temp.GetComponentInChildren<RawImage>().texture); } );
        }
    }
}
