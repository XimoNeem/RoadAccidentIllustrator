using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RoadAccidentIllustrator.RAI_ObjectSettings;

[System.Serializable]
public class RAI_RoadPoint : RAI_Road
{
    public float wigth = 2;
    public float pointsFrequency = 5f;
    public Material roadMat;
    [Space]
    public bool isParent;
    public RAI_RoadPoint parentPoint;
    public RAI_RoadPoint connectedPoint;
    public float cornerGap = 2;
    public float dustBorderSize = 2;

    public float borderHeight = 0.15f;
    public float borderDepth = 0.15f;
    public float borderDegree = 0.04f;
    public float sidewalkWidth = 2.5f;

    public float dumperBaseSclae = 0.5f;
    public float dumperLineHeight = 1f;
    public float dumperLineWidth = 1f;
    public float dumperDegree = 0.25f;
    public float dumperLineForwardOffset = 0.25f;

    List<GameObject> dumperBase = new List<GameObject>();
    List<GameObject> debugPoints = new List<GameObject>();

    public float overlapRadius = 2;

    public GameObject dumperBaseTemplate;
    [Space]
    List<List<Transform>> bordersPath = new List<List<Transform>>();
    List<List<Transform>> roadsPath = new List<List<Transform>>();
    [Space]
    public MeshRenderer roadRenderer;
    public MeshRenderer borderRenderer;
    public MeshRenderer dustBorderRenderer;
    public MeshRenderer dumperRenderer;
    public MeshRenderer centerDumperRenderer;
    public MeshRenderer sideWalkRenderer;
    [Space]
    public List<RAI_RoadPoint> chilsPoints;

    private IEnumerator updater;
    private List<Transform> roadDrawPoints;

    private void OnEnable()
    {
        EventBus.OnObjectMove += ChangeTransform;
        EventBus.OnObjectMove += CheckConnectors;
        EventBus.OnObjectRotate += ChangeTransform;
        EventBus.OnObjectDeselected += HidePoints;
        EventBus.OnObjectSelected += HidePoints;
    }

    private void OnDisable()
    {
        EventBus.OnObjectMove -= ChangeTransform;
        EventBus.OnObjectMove -= CheckConnectors;
        EventBus.OnObjectRotate -= ChangeTransform;
        EventBus.OnObjectDeselected -= HidePoints;
        EventBus.OnObjectSelected -= HidePoints;
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

        if (isParent)
        {
            foreach (var item in chilsPoints)
            {
                item.objectSettings = this.objectSettings;
            }
        }

        else
        {
            parentPoint.objectSettings = this.objectSettings;
            foreach (var item in parentPoint.chilsPoints)
            {
                if (item != this)
                {
                    item.objectSettings = this.objectSettings;
                }
            }
        }

        BeginUpdate();
    }

    public override void OnMoved()
    {
        base.OnMoved();

        BeginUpdate();
    }

    public override void OnRotated()
    {
        base.OnRotated();

        BeginUpdate();
    }

    public void ClearRoad()
    {
        foreach (var item in debugPoints)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        debugPoints.Clear();

        foreach (var item in dumperBase)
        {
            Destroy(item);
        }
        dumperBase.Clear();

        roadRenderer.GetComponent<MeshFilter>().mesh.Clear();
        borderRenderer.GetComponent<MeshFilter>().mesh.Clear();
        dustBorderRenderer.GetComponent<MeshFilter>().mesh.Clear();
        dumperRenderer.GetComponent<MeshFilter>().mesh.Clear();
        sideWalkRenderer.GetComponent<MeshFilter>().mesh.Clear();
        centerDumperRenderer.GetComponent<MeshFilter>().mesh.Clear();
    }

    void AddPoint()
    {
        CheckConnectors();

        foreach (var item in FindObjectsOfType<RAI_RoadPoint>())
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
            foreach(var item in FindObjectsOfType<RAI_RoadPoint>())
            {
                item.selected = false;
            }
            selected = true;
        }
    }

    public override void OnDeselected()
    {
        base.OnDeselected();

        //selected = false;
    }

    public void SetConnectedPoint(RAI_RoadPoint point)
    {
        connectedPoint = point;
        //Debug.Log("SetConnectedPointFront");
    }

    public IEnumerator RefreshRoad()
    {
        DestroyPoints();

        ClearRoad();

        List<Transform> right_border = new List<Transform>();
        List<Transform> left_border = new List<Transform>();


        int width = SettingsManager.GetIntValue(this, settingItemType.road_width);
        roadWidth type = new roadWidth();
        type = (roadWidth)width;
        float wid = SettingsManager.GetRoadWidth(type);

        Transform l_start = new GameObject("l_start").transform;
        Transform l_end = new GameObject("l_end").transform;
        Transform r_start = new GameObject("r_start").transform;
        Transform r_end = new GameObject("r_end").transform;

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < chilsPoints.Count; i++)
        {
            roadsPath.Add(new List<Transform>());

            if (i == 0 && chilsPoints.Count != 0)
            {
                roadDrawPoints.AddRange(getConnnectedPoints(this.transform, chilsPoints[i].transform, true));
                roadsPath[i].AddRange(getConnnectedPoints(this.transform, chilsPoints[i].transform, true));

                r_start.position = this.transform.position + this.transform.right * wid;
                r_start.rotation = this.transform.rotation;
                l_start.position = this.transform.position + this.transform.right * -wid;
                l_start.rotation = this.transform.rotation;

                r_end.position = chilsPoints[i].transform.position + chilsPoints[i].transform.right * wid;
                r_end.rotation = chilsPoints[i].transform.rotation;
                l_end.position = chilsPoints[i].transform.position + chilsPoints[i].transform.right * -wid;
                l_end.rotation = chilsPoints[i].transform.rotation;

                //left_border.Add(l_start);
                //right_border.Add(l_start);

                left_border.AddRange(getConnnectedPoints(l_start, l_end, true));
                right_border.AddRange(getConnnectedPoints(r_start, r_end, true));
            }
            else
            {
                roadDrawPoints.AddRange(getConnnectedPoints(chilsPoints[i - 1].transform, chilsPoints[i].transform, true));
                roadsPath[i].AddRange(getConnnectedPoints(chilsPoints[i - 1].transform, chilsPoints[i].transform, true));

                r_start.position = chilsPoints[i - 1].transform.position + chilsPoints[i - 1].transform.right * wid;
                r_start.rotation = chilsPoints[i - 1].transform.rotation;
                l_start.position = chilsPoints[i - 1].transform.position + chilsPoints[i - 1].transform.right * -wid;
                l_start.rotation = chilsPoints[i - 1].transform.rotation;

                r_end.position = chilsPoints[i].transform.position + chilsPoints[i].transform.right * wid;
                r_end.rotation = chilsPoints[i].transform.rotation;
                l_end.position = chilsPoints[i].transform.position + chilsPoints[i].transform.right * -wid;
                l_end.rotation = chilsPoints[i].transform.rotation;

                left_border.AddRange(getConnnectedPoints(l_start, l_end, false));
                right_border.AddRange(getConnnectedPoints(r_start, r_end, false));
            }
        }


        left_border[0].rotation = this.transform.rotation;
        right_border[0].rotation = this.transform.rotation;

        right_border = inverseList(right_border);

        foreach (var item in left_border)
        {
            item.eulerAngles += new Vector3(0, -90, 0);
        }

        foreach (var item in right_border)
        {
            item.eulerAngles += new Vector3(0, 90, 0);
        }

        bordersPath.Clear();
        bordersPath.Add(new List<Transform>());
        bordersPath.Add(new List<Transform>());

        bordersPath[0] = left_border;
        bordersPath[1] = right_border;


        yield return new WaitForEndOfFrame();

        for (int i = 0; i < roadDrawPoints.Count; i++)
        {
            if (i + 1 == roadDrawPoints.Count)
            {
                roadDrawPoints[i].LookAt(this.transform);
            }
            else
            {
                roadDrawPoints[i].LookAt(roadDrawPoints[i + 1]);
            }
        }

        yield return new WaitForEndOfFrame();

        GenerateRoad();


        if (SettingsManager.GetBoolValue(this, settingItemType.road_border) == true)
        {
            AddBorder();
            AddSidewalk();
        }
        if (SettingsManager.GetBoolValue(this, settingItemType.road_dustborder) == true)
        {
            AddDustBorder();
        }
        if (SettingsManager.GetBoolValue(this, settingItemType.road_bumpborder) == true)
        {
            AddDumper();
        }
        if (SettingsManager.GetBoolValue(this, settingItemType.road_bumpborder_center) == true)
        {
            AddCenterDumper();
        }

        yield return new WaitForEndOfFrame();

        foreach (var listItem in bordersPath)
        {
            foreach (var item in listItem)
            {
                //Destroy(item.gameObject);
            }
        }

        foreach (var item in roadDrawPoints)
        {
            if(item.gameObject != null)
            Destroy(item.gameObject);
        }
        foreach (var listItem in roadsPath)
        {
            foreach (var item in listItem)
            {
                Destroy(item.gameObject);
            }
        }
        roadsPath.Clear();

        Destroy(l_start.gameObject);
        Destroy(l_end.gameObject);
        Destroy(r_start.gameObject);
        Destroy(r_end.gameObject);

        RefreshCollision();
    }

    public void DrawDebugPoint(Vector3 pos, Color color)
    {
        GameObject debugPoint = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debugPoint.transform.position = pos;
        debugPoint.transform.eulerAngles += new Vector3(90, 0, 0);

        debugPoint.GetComponent<MeshRenderer>().material.color = color;
        debugPoint.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.transform.localScale *= 0.3f;
        debugPoint.gameObject.layer = 9;
        debugPoint.transform.parent = GameObject.Find("DebugPoints").transform;

        debugPoints.Add(debugPoint.gameObject);
    }

    public void DrawDebugPoint(Vector3 pos, Color color, string name)
    {
        GameObject debugPoint = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debugPoint.transform.position = pos;
        debugPoint.transform.eulerAngles += new Vector3(90, 0, 0);

        debugPoint.name = name;

        debugPoint.GetComponent<MeshRenderer>().material.color = color;
        debugPoint.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.transform.localScale *= 0.3f;
        debugPoint.gameObject.layer = 9;
        debugPoint.transform.parent = GameObject.Find("DebugPoints").transform;

        debugPoints.Add(debugPoint.gameObject);
    }

    private List<Transform> inverseList(List<Transform> list)
    {
        List<Transform> result = new List<Transform>();

        for (int i = 0; i < list.Count; i++)
        {
            result.Add(list[list.Count - (i + 1)]);
        }

        return result;
    }

    private List<Transform> getConnnectedPoints(Transform a, Transform b, bool addLastPoints)
    {
        float distance = Vector3.Distance(a.position, b.position);
        int pointsAmount = Mathf.RoundToInt(distance / pointsFrequency);

        List<Transform> result = getBorderPoints(pointsAmount, distance, a, b, addLastPoints, false, true, false);

        return result;
    }

    private List<Transform> getConnnectedPoints(Transform a, Transform b, bool addLastPoints, bool addFirstPoints)
    {
        float distance = Vector3.Distance(a.position, b.position);
        int pointsAmount = Mathf.RoundToInt(distance / pointsFrequency);

        List<Transform> result = getBorderPoints(pointsAmount, distance, a, b, addLastPoints, false, addFirstPoints, false);

        return result;
    }

    private List<Transform> getConnnectedPoints(Transform a, Transform b, bool addLastPoints, bool addFirstPoints, bool setRotation)
    {
        float distance = Vector3.Distance(a.position, b.position);
        int pointsAmount = Mathf.RoundToInt(distance / pointsFrequency);

        List<Transform> result = getBorderPoints(pointsAmount, distance, a, b, addLastPoints, false, addFirstPoints, setRotation);

        return result;
    }

    private List<Transform> getBorderPoints(int pointsAmount, float distance, Transform a, Transform b, bool addLastPoints, bool drawDebug, bool addFirstPoints, bool setRotation)
    {
        List<Transform> result = new List<Transform>();

        Vector3 target = a.position;

        Vector3 Aleft = a.position + a.right * (sidewalkWidth + borderDepth);
        Vector3 Bleft = b.position + b.right * (sidewalkWidth + borderDepth);

        if (setRotation)
        {
            //DrawDebugPoint(Aleft, Color.green);
            //DrawDebugPoint(Bleft, Color.white);
        }


        if (addLastPoints)
        {
            Transform startPoint = new GameObject().transform;
            startPoint.position = Vector3.Lerp(a.position, b.position, 0);
            startPoint.rotation = a.rotation;
            startPoint.gameObject.name = "startPoint";
            result.Add(startPoint);

            //Debug.Log("startPoint");
        }

        for (int i = 1; i < pointsAmount; i++)
        {

            float t = (float)i / (float)pointsAmount;


            Transform point = new GameObject().transform;
            point.position = Vector3.Lerp(a.position, b.position, t);
            point.gameObject.name = "point[" + i + "]";

            Vector3 a_fv = a.position + (a.forward * (distance * t));
            Vector3 b_fv = b.position + (b.forward * (distance * (-1 + t)));

            point.position = Vector3.Lerp(a_fv, point.position, t);
            point.position = Vector3.Lerp(point.position, b_fv, t);

            //point.position = Vector3.Lerp(a_fv, b_fv, t);

            //point.LookAt(Vector3.Lerp(Aleft, Bleft, t));
            point.eulerAngles += new Vector3(0, 180, 0);

            result.Add(point);
            target = point.position;

            if (setRotation)
            {
                point.LookAt(Vector3.Lerp(Aleft, Bleft, t));
                point.eulerAngles += new Vector3(0, -90, 0);
            }
        }

        if (addFirstPoints)
        {
            Transform endPoint = new GameObject().transform;
            endPoint.position = Vector3.Lerp(a.position, b.position, 1);
            endPoint.rotation = b.rotation;
            //endPoint.eulerAngles += new Vector3(0, 180, 0);
            endPoint.gameObject.name = "endPoint";
            result.Add(endPoint);
        }

        if (!setRotation)
        {
            for (int i = 0; i < result.Count; i++)
            {
                if (i < result.Count - 1)
                {
                    result[i].LookAt(result[i + 1]);
                }
            }
        }


        if (drawDebug)
        {
            foreach (var item in result)
            {
                //DrawDebugPoint(item, Color.black);
            }
        }

        return result;
    }

    private void GenerateRoad()
    {
        List<Vector2> drawVectors = new List<Vector2>();

        foreach (var listItem in bordersPath)
        {
            foreach (var item in listItem)
            {
                Vector2 point = new Vector2(item.position.x - this.transform.position.x, item.position.z - this.transform.position.z);
                drawVectors.Add(point);
            }
        }

        drawVectors.Add(drawVectors[0]);

        List<Vector3> vert = null;
        List<int> tri = null;
        //Triangulation.GetResult(drawVectors, true, Vector3.up, out vert, out tri, out uv); 

        RAI_Triangulation.Triangulate(drawVectors, out tri, out vert);

        for (int i = 0; i < vert.Count; i++)
        {
            Vector3 newPos = Quaternion.Euler(0, -this.transform.eulerAngles.y, 0) * vert[i];
            vert[i] = newPos;
        }

        Vector2[] uvs = new Vector2[vert.Count];

        for (int i = 0; i < vert.Count; i++)
        {
            uvs[i] = new Vector2(vert[i].x, vert[i].z);
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vert.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs;
        mesh.Optimize();
        mesh.RecalculateNormals();



        drawVectors.Clear();


        roadRenderer.GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void ChangeTransform()
    {
        if (FindObjectOfType<ObjectMover>().selectedObjects.Contains(this.transform))
        {
            //BeginUpdate();
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

        RefreshCollision();
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

                float degree = Vector3.SignedAngle(this.transform.forward, hitCollider.transform.forward, Vector3.up);

                if (Mathf.Abs(degree) < 180)
                {
                    this.transform.rotation = hitCollider.transform.rotation;
                }
                else if (Mathf.Abs(degree) > 180)
                {
                    this.transform.rotation = hitCollider.transform.rotation;
                    this.transform.eulerAngles += new Vector3(0, 180, 0);

                }
                else
                {
                    this.transform.rotation = hitCollider.transform.rotation;
                }

                if (isParent)
                {
                    this.transform.eulerAngles += new Vector3(0, 180, 0);
                }
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
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        roadDrawPoints.Clear();
    }

    private void AddBorder()
    {
        List<Vector3> borderTops = new List<Vector3>();
        List<Vector3> sidewalkStarts = new List<Vector3>();

        List<Vector3> points = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tri = new List<int>();
        int triIndex = 0;
        float uvOffset = 0;
        float dustOffset = 0.5f;

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = item[i].position - this.transform.position;

                points.Add(newPoint);
                uvs.Add(new Vector2(0, uvOffset));

                newPoint = (item[i].position + item[i].up * borderHeight + item[i].forward * borderDegree) - this.transform.position;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in bordersPath)
        {
            uvOffset = 0;
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * borderHeight + item[i].forward * borderDegree) - this.transform.position;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * borderHeight + item[i].forward * borderDepth) - this.transform.position;

                points.Add(newPoint);
                uvs.Add(new Vector2(1f, uvOffset));

                border.Add((item[i].position + item[i].up * borderHeight) - this.transform.position);
                border.Add((item[i].position + item[i].up * borderHeight + item[i].forward * borderDepth) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 newPos = Quaternion.Euler(0, -this.transform.eulerAngles.y, 0) * points[i];
            points[i] = newPos;
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        borderRenderer.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AddSidewalk()
    {
        List<Vector3> borderTops = new List<Vector3>();
        List<Vector3> sidewalkStarts = new List<Vector3>();

        List<Vector3> points = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tri = new List<int>();
        int triIndex = 0;
        float uvOffset = 0;
        float dustOffset = 0.5f;

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;
                newPoint = (item[i].position + item[i].up * borderHeight + item[i].forward * borderDepth) - this.transform.position;

                points.Add(newPoint);
                uvs.Add(new Vector2(newPoint.x, newPoint.z));
                newPoint = (item[i].position + item[i].up * borderHeight + item[i].forward * sidewalkWidth) - this.transform.position;
                points.Add(newPoint);
                uvs.Add(new Vector2(newPoint.x, newPoint.z));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 newPos = Quaternion.Euler(0, -this.transform.eulerAngles.y, 0) * points[i];
            points[i] = newPos;
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        sideWalkRenderer.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AddDustBorder()
    {
        List<Vector3> points = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tri = new List<int>();
        int triIndex = 0;
        float uvOffset = 0;
        float dustOffset = 0.5f;

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;
                points.Add((item[i].position + item[i].forward * -dustOffset) - this.transform.position);
                uvs.Add(new Vector2(0, uvOffset));
                points.Add((item[i].position + item[i].forward * dustBorderSize) - this.transform.position);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 newPos = Quaternion.Euler(0, -this.transform.eulerAngles.y, 0) * points[i];
            points[i] = newPos;
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        dustBorderRenderer.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AddDumper()
    {
        if (dumperBaseTemplate == null)
        {
            return;
        }

        foreach (var listItem in bordersPath)
        {
            foreach (var item in listItem)
            {
                GameObject obj = Instantiate(dumperBaseTemplate, item.position, item.rotation, dumperRenderer.transform);
                obj.transform.eulerAngles += new Vector3(-90, 0, 90);
                obj.transform.localScale *= dumperBaseSclae;

                dumperBase.Add(obj);
            }
        }

        List<Vector3> points = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tri = new List<int>();
        int triIndex = 0;
        float uvOffset = 0;
        float dustOffset = 0.5f;

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = item[i].position - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.25f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.25f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.5f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.5f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.75f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in bordersPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.75f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 1) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 newPos = Quaternion.Euler(0, -this.transform.eulerAngles.y, 0) * points[i];
            points[i] = newPos;
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        dumperRenderer.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AddCenterDumper()
    {
        float doubleOffset = 2;

        if (dumperBaseTemplate == null)
        {
            return;
        }

        foreach (var listItem in roadsPath)
        {
            foreach (var item in listItem)
            {
                item.eulerAngles += new Vector3(0, 90, 0);
                GameObject obj = Instantiate(dumperBaseTemplate, item.position, item.rotation, dumperRenderer.transform);
                obj.transform.eulerAngles += new Vector3(-90, 0, 90);
                obj.transform.localScale *= dumperBaseSclae;

                dumperBase.Add(obj);
            }
        }

        List<Vector3> points = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tri = new List<int>();
        int triIndex = 0;
        float uvOffset = 0;
        float dustOffset = 0.5f;

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = item[i].position - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.25f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.25f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.5f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.5f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.75f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.75f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 1) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * dumperLineForwardOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = item[i].position - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.25f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.25f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.5f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.5f) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.75f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        foreach (var item in roadsPath)
        {
            List<Vector3> border = new List<Vector3>();

            Vector3 newPoint = Vector3.zero;

            for (int i = 0; i < item.Count; i++)
            {
                uvOffset += 1;

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 0.75f) + item[i].forward * dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(0.5f, uvOffset));

                newPoint = (item[i].position + item[i].up * (dumperLineWidth * 1) + item[i].forward * -dumperDegree) - this.transform.position;
                newPoint += Vector3.up * dumperLineHeight;
                newPoint += item[i].forward * -dumperLineForwardOffset * doubleOffset;

                points.Add(newPoint);
                uvs.Add(new Vector2(1, uvOffset));

                border.Add(item[i].position - this.transform.position);
                border.Add(((item[i].position + item[i].forward * dustBorderSize) + item[i].forward * dustOffset) - this.transform.position);
            }

            tri.AddRange(ConnectLines(border, triIndex));

            triIndex += border.Count;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 newPos = Quaternion.Euler(0, -this.transform.eulerAngles.y, 0) * points[i];
            points[i] = newPos;
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        centerDumperRenderer.GetComponent<MeshFilter>().mesh = mesh;
    }

    private List<int> ConnectLines(List<Vector3> lineA, int triangleOffset)
    {
        List<int> result = new List<int>();

        int length = lineA.Count - 2;

        //Debug.Log(triangleOffset);

        for (int i = 0; i < length; i++)
        {
            if (i % 2 == 0)
            {
                result.Add(triangleOffset + i);
                result.Add(triangleOffset + i + 1);
                result.Add(triangleOffset + i + 2);
            }
            else if (i == 0)
            {
                result.Add(triangleOffset + i);
                result.Add(triangleOffset + i + 2);
                result.Add(triangleOffset + i + 1);
            }
            else
            {
                result.Add(triangleOffset + i);
                result.Add(triangleOffset + i + 2);
                result.Add(triangleOffset + i + 1);
            }
        }

        return result;
    }

    private void RefreshCollision()
    {
        if(roadRenderer.gameObject.GetComponent<MeshCollider>())
            Destroy(roadRenderer.gameObject.GetComponent<MeshCollider>());
        if (borderRenderer.gameObject.GetComponent<MeshCollider>())
            Destroy(borderRenderer.gameObject.GetComponent<MeshCollider>());
        if (dustBorderRenderer.gameObject.GetComponent<MeshCollider>())
            Destroy(dustBorderRenderer.gameObject.GetComponent<MeshCollider>());
        if (dumperRenderer.gameObject.GetComponent<MeshCollider>())
            Destroy(dumperRenderer.gameObject.GetComponent<MeshCollider>());
        if (centerDumperRenderer.gameObject.GetComponent<MeshCollider>())
            Destroy(centerDumperRenderer.gameObject.GetComponent<MeshCollider>());
        if (sideWalkRenderer.gameObject.GetComponent<MeshCollider>())
            Destroy(sideWalkRenderer.gameObject.GetComponent<MeshCollider>());

        roadRenderer.gameObject.AddComponent<MeshCollider>();
        borderRenderer.gameObject.AddComponent<MeshCollider>();
        dustBorderRenderer.gameObject.AddComponent<MeshCollider>();
        dumperRenderer.gameObject.AddComponent<MeshCollider>();
        centerDumperRenderer.gameObject.AddComponent<MeshCollider>();
        sideWalkRenderer.gameObject.AddComponent<MeshCollider>();
    }
}
