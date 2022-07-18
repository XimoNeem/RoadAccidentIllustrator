using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_Mover;
using RoadAccidentIllustrator.RAI_Events;

public class GizmoItemHelper : MonoBehaviour
{
    public MoveItemType itemType;
    public LayerMask objectControl;
    public Vector3 offset;
    public int rotationOffset;
    private Transform parent;
    private ObjectMover mover;
    private Ray ray;
    private RaycastHit hit = new RaycastHit();

    
    private void Awake()
    {
        parent = FindObjectOfType<GizmoParentHelper>().transform;
        if (FindObjectOfType<ObjectMover>())
        {
            mover = FindObjectOfType<ObjectMover>();
        }
        else
        {
            RAI_DebugManager.instance.ShowMessage("NO MOOVER FOUND", Color.red);
        }
    }
    private void OnMouseDown()
    {
        if (itemType == MoveItemType.Info)
        {
            EventBus.OnObjectInfo?.Invoke();
        }
    }

    private void OnMouseUp()
    {
        if (itemType == MoveItemType.Move)
        {
            foreach (var item in mover.selectedObjects)
            {
                if (item.GetComponent<Movable>())
                {
                    item.GetComponent<Movable>().OnMovedEnd();
                }
            }
        }

        else if (itemType == MoveItemType.Rotate)
        {
            foreach (var item in mover.selectedObjects)
            {
                if (item.GetComponent<Movable>())
                {
                    item.GetComponent<Movable>().OnRotatedEnd();
                }
            }
        }
    }

    private void OnMouseDrag()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (itemType == MoveItemType.Move)
        {
            if (Physics.Raycast(ray, out hit, 500, objectControl))
            {
                //Vector3 moveOffset = this.transform.forward * 1.5f;
                Vector3 newPos = Vector3.zero;
                newPos = hit.point - offset * this.transform.localScale.z;
                //newPos.y = parent.transform.position.y;
                parent.position = newPos;

                EventBus.OnObjectMove?.Invoke();

                foreach (var item in mover.selectedObjects)
                {
                    if (item.GetComponent<Movable>())
                    {
                        item.GetComponent<Movable>().OnMoved();
                    }
                }
            }
        }

        else if (itemType == MoveItemType.Rotate)
        {
            if (Physics.Raycast(ray, out hit, 500, objectControl))
            {
                Vector3 pos = parent.eulerAngles;
                parent.LookAt(hit.point);
                parent.eulerAngles = new Vector3(0, parent.eulerAngles.y + rotationOffset, 0);

                EventBus.OnObjectRotate?.Invoke();

                foreach (var item in mover.selectedObjects)
                {
                    if (item.GetComponent<Movable>())
                    {
                        item.GetComponent<Movable>().OnRotated();
                    }
                }
            }       
        }
    }
}
