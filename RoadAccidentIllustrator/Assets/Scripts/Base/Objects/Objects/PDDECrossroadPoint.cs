using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDDECrossroadPoint : PDDERoad
{
    public RAI_CrossroadCenter parent;
    public PDDECrossroadPoint connectedPoint;

    public float overlapRadius = 2;

    void OnDisable()
    {
        EventBus.OnObjectMove += CheckConnectors;
    }

    void OnEnable()
    {
        EventBus.OnObjectMove -= CheckConnectors;
    }
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        UpdateConnectedPointsSettings();

        parent.ClearRoad();
        parent.RefreshRoad();
    }

    public override void OnMovedEnd()
    {
        base.OnMoved();
        parent.ClearRoad();
        parent.RefreshRoad();
    }

    public override void OnRotatedEnd()
    {
        base.OnRotated();
        parent.ClearRoad();
        parent.RefreshRoad();
    }

    public override void OnMoved()
    {
        base.OnMoved();
        //parent.ClearRoad();

        //CheckConnectors();
    }

    public override void OnRotated()
    {
        base.OnRotated();
        //parent.ClearRoad();
    }

    private void UpdateConnectedPointsSettings()
    {
        connectedPoint.objectSettings = this.objectSettings;
    }

    private void CheckConnectors()
    {
        if (!FindObjectOfType<ObjectMover>().selectedObjects.Contains(this.transform))
        {
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, overlapRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<PDDERoadConnector>())
            {
                this.transform.position = hitCollider.transform.position;
                this.transform.rotation = hitCollider.transform.rotation;
            }
        }
    }
}
