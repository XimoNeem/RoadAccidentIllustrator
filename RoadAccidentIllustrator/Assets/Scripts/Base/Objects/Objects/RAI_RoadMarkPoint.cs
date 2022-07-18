using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadAccidentIllustrator.RAI_ObjectSettings;

public class RAI_RoadMarkPoint : RAI_RoadMark
{
    public Texture2D tex0;
    public Texture2D tex1;
    public Texture2D tex2;
    public Texture2D tex3;
    public Texture2D tex4;
    [Space]
    public RoadMarkPointSettings settings;
    [Space]
    public float wigth = 2;
    public float pointsFrequency = 5f;
    public Material roadMat;
    [Space]
    public bool isParent;
    public RAI_RoadMarkPoint parentPoint;
    public RAI_RoadMarkPoint connectedPoint;
    [Space]
    public List<RAI_RoadMarkPoint> chilsPoints;
    

    private IEnumerator updater;
    private List<Transform> roadDrawPoints;
    private float overlapRadius = 2;
    private float offset = 0.01f;

    private void OnEnable()
    {
        EventBus.OnObjectMove += ChangeTransform;
        EventBus.OnObjectMove += CheckConnectors;
        EventBus.OnObjectRotate += ChangeTransform;
        EventBus.OnObjectInfo += ChangeTransform;

        EventBus.OnObjectDeselected += HidePoints;
        EventBus.OnObjectSelected += HidePoints;
    }

    private void OnDisable()
    {
        EventBus.OnObjectMove  -=  ChangeTransform;
        EventBus.OnObjectMove  -=  CheckConnectors;
        EventBus.OnObjectRotate  -=  ChangeTransform;
        EventBus.OnObjectInfo  -=  ChangeTransform;

        EventBus.OnObjectDeselected  -=  HidePoints;
        EventBus.OnObjectSelected  -=  HidePoints;
    }
    public override void Start()
    {
        base.Start();

        roadDrawPoints = new List<Transform>();

        if (editable)
        {
            AddPoint();
        }
    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        List<RAI_RoadMarkPoint> pointsToChanges = new List<RAI_RoadMarkPoint>();

        if (!isParent)
        {
            pointsToChanges.Add(parentPoint);
            pointsToChanges.AddRange(parentPoint.chilsPoints);
        }
        else if(isParent)
        {
            pointsToChanges.Add(this);
            pointsToChanges.AddRange(chilsPoints);
        }

        pointsToChanges.Remove(this);

        foreach (var item in pointsToChanges)
        {
            item.objectSettings = this.objectSettings;

            //item.BeginUpdate();
        }
        BeginUpdate();
    }

    void AddPoint()
    {
        CheckConnectors();

        foreach (var item in FindObjectsOfType<RAI_RoadMarkPoint>())
        {
            if (item.selected)
            {
                this.SetConnectedPoint(item);

                connectedPoint.transform.LookAt(this.transform);

                item.selected = false;
                break;
            }
        }

        if (connectedPoint == null)
        {
            isParent = true;
        }
        else
        {
            if (!connectedPoint.isParent)
            {
                if (connectedPoint.parentPoint != null)
                {
                    parentPoint = connectedPoint.parentPoint;
                }
            }
            else
            {
                parentPoint = connectedPoint;
            }
        }

        if (parentPoint != null)
        {
            parentPoint.chilsPoints.Add(this);
        }


        selected = true;
        editable = true;

        BeginUpdate();
    }

    public override void OnSelected()
    {
        base.OnSelected();

        if (editable)
        {
            foreach (var item in FindObjectsOfType<RAI_RoadMarkPoint>())
            {
                item.selected = false;
            }
            selected = true;
            Debug.Log("selected");
        }
    }

    public override void OnDeselected()
    {
        base.OnDeselected();

        //selected = false;
    }

    public void SetConnectedPoint(RAI_RoadMarkPoint point)
    {
        connectedPoint = point;
    }

    public IEnumerator RefreshRoad()
    {
        DestroyPoints();
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < chilsPoints.Count; i++)
        {
            if (i == 0 & chilsPoints.Count != 0)
            {
                roadDrawPoints.AddRange(getConnnectedPoints(this.transform, chilsPoints[i].transform));
            }
            else
            {
                roadDrawPoints.AddRange(getConnnectedPoints(chilsPoints[i - 1].transform, chilsPoints[i].transform));
            }
        }
        yield return new WaitForEndOfFrame();
        GenerateRoad(roadDrawPoints);
    }

    private List<Transform> getConnnectedPoints(Transform a, Transform b)
    {
        float distance = Vector3.Distance(a.position, b.position);
        int pointsAmount = Mathf.RoundToInt(distance / pointsFrequency);

        List<Transform> result = getBorderPoints(pointsAmount, distance, a, b);

        return result;
    }

    private List<Transform> getBorderPoints(int pointsAmount, float distance, Transform a, Transform b)
    {
        List<Transform> result = new List<Transform>();

        Transform startPoint = new GameObject().transform;
        startPoint.position = Vector3.Lerp(a.position, b.position, 0);
        startPoint.position += new Vector3(0, offset, 0);
        result.Add(startPoint);

        for (int i = 1; i < pointsAmount; i++)
        {
            float t = (float)i / (float)pointsAmount;
            Transform point = new GameObject().transform;
            
            point.position = Vector3.Lerp(a.position, b.position, t);

            Vector3 a_fv = a.position + (a.forward * (distance * t));
            Vector3 b_fv = b.position + (b.forward * (distance * (-1 + t)));

            point.position = Vector3.Lerp(a_fv, point.position, t);
            point.position = Vector3.Lerp(point.position, b_fv, t);
            point.position += new Vector3(0, offset, 0);

            result.Add(point);
        }

        Transform endPoint = new GameObject().transform;
        endPoint.position = Vector3.Lerp(a.position, b.position, 1);
        endPoint.position += new Vector3(0, offset, 0);
        result.Add(endPoint);

        return result;
    }

    private void GenerateRoad(List<Transform> roadPoints)
    {
        MeshFilter filter;
        if (this.gameObject.GetComponent<MeshFilter>())
        {
            filter = this.gameObject.GetComponent<MeshFilter>();
        }
        else { filter = this.gameObject.AddComponent<MeshFilter>(); }

        MeshRenderer renderer;
        if (this.gameObject.GetComponent<MeshRenderer>())
        {
            renderer = this.gameObject.GetComponent<MeshRenderer>();
        }
        else { renderer = this.gameObject.AddComponent<MeshRenderer>(); }

        LineRenderer line;
        if (this.gameObject.GetComponentInChildren<LineRenderer>())
        {
            line = this.gameObject.GetComponentInChildren<LineRenderer>();
        }
        else { line = this.gameObject.AddComponent<LineRenderer>(); }

        settings.textureIndex = SettingsManager.GetIntValue(this, settingItemType.roadmark_roadmarkline);
        settings.width = SettingsManager.GetFloatValue(this, settingItemType.roadmark_roadmarklinewidth);
        settings.color = SettingsManager.GetColorValue(this, settingItemType.roadmark_roadmarklinecolor);

        line.material = roadMat;

        switch (settings.textureIndex)
        {
            case 0:
                line.material.mainTexture = tex0;
                break;
            case 1:
                line.material.mainTexture = tex1;
                break;
            case 2:
                line.material.mainTexture = tex2;
                break;
            case 3:
                line.material.mainTexture = tex3;
                break;
            case 4:
                line.material.mainTexture = tex4;
                break;
            default:
                break;
        }

        line.material.color = settings.color;
        line.material.color = settings.color;
        line.startWidth = settings.width;
        line.endWidth = settings.width;

        line.positionCount = roadPoints.Count;
        for (int i = 0; i < roadPoints.Count; i++)
        {
            line.SetPosition(i, roadPoints[i].position);
        }
    }

    private void ChangeTransform()
    {
        if (FindObjectOfType<ObjectMover>().selectedObjects.Contains(this.transform))
        {
            BeginUpdate();
        }
    }

    private void BeginUpdate()
    {
        if (isParent)
        {
            updater = RefreshRoad();
            StartCoroutine(updater);
        }
        else if (!isParent)
        {
            parentPoint.BeginUpdate();
        }
    }

    private void CheckConnectors()
    {
        if (!FindObjectOfType<ObjectMover>().selectedObjects.Contains(this.transform) || this.editable == false)
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

    private void HidePoints()
    {
        if (this.isParent && this.selected == false)
        {
            foreach (var item in chilsPoints)
            {
                if (item.selected)
                {
                    return;
                }
            }
            DestroyPoints();
        }
    }

    private void DestroyPoints()
    {
        foreach (var item in roadDrawPoints)
        {
            //item.GetComponent<MeshRenderer>().enabled = false;
            Destroy(item.gameObject);
        }

        roadDrawPoints.Clear();
    }
}
