using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RoadAccidentIllustrator.RAI_ObjectSettings;
//using TriangleNet;
//using TriangleNet.Geometry;

public class RAI_CrossroadCenter : PDDERoad
{
    [Header("Добавлять точки последовательно, по часовой стрелке!")]
    public List<PDDECrossroadPoint> roadPoints;
    public RAI_CrossroadParent parent;
    public bool drawDebug = false;
    public bool roundedCorners = true;
    private float debugSize = 0.25f;
    [Space]
    public float pointsFrequency = 5f;
    public float cornerGap = 2;
    public float dustBorderSize = 2;

    public float borderHeight = 0.15f;
    public float borderDepth= 0.15f;
    public float borderDegree = 0.04f;
    public float sidewalkWidth = 2.5f;

    public float dumperBaseSclae = 0.5f;
    public float dumperLineHeight = 1f;
    public float dumperLineWidth = 1f;
    public float dumperDegree = 0.25f;
    public float dumperLineForwardOffset = 0.25f;
    [Space]
    List<List<Transform>> roadsPath = new List<List<Transform>>();
    List<List<Transform>> bordersPath = new List<List<Transform>>();
    List<List<Transform>> borderPathLeft = new List<List<Transform>>();
    List<List<Transform>> borderPathRight = new List<List<Transform>>();
    List<List<Transform>> cornerPath = new List<List<Transform>>();

    List<GameObject> dumperBase = new List<GameObject>();
    [Space]
    public MeshRenderer roadRenderer;
    public MeshRenderer borderRenderer;
    public MeshRenderer dustBorderRenderer;
    public MeshRenderer dumperRenderer;
    public MeshRenderer centerDumperRenderer;
    public MeshRenderer sideWalkRenderer;
    [Space]
    List<Transform> debugPoints = new List<Transform>();
    public List<Vector2> drawVectors;
    public GameObject dumperBaseTemplate;

    public override void Start()
    {
        base.Start();
        foreach (var item in roadPoints)
        {
            item.parent = this;
            item.connectedPoint.parent = this;
        }
        RefreshRoad();
    }

    public void RefreshRoad()
    {
        Refresh();
    }

    public override void ApplySettings()
    {
        base.ApplySettings();
        RefreshRoad();
    }

    public override void OnMovedEnd()
    {
        base.OnMoved();
        RefreshRoad();
    }

    public override void OnRotatedEnd()
    {
        base.OnRotated();
        RefreshRoad();
    }

    public override void OnMoved()
    {
        base.OnMoved();
    }

    public override void OnRotated()
    {
        base.OnRotated();
    }

    public void ClearRoad()
    {
        foreach (var item in bordersPath)
        {
            foreach (var listItem in item)
            {
                if (listItem != null)
                {
                    Destroy(listItem.gameObject);
                }
            }
            item.Clear();
        }
        bordersPath.Clear();

        foreach (var item in borderPathLeft)
        {
            foreach (var listItem in item)
            {
                if (listItem != null)
                {
                    Destroy(listItem.gameObject);
                }
            }
            item.Clear();
        }
        borderPathLeft.Clear();

        foreach (var item in roadsPath)
        {
            foreach (var listItem in item)
            {
                if (listItem != null)
                {
                    if (listItem != null)
                    {
                        Destroy(listItem.gameObject);
                    }
                }
            }
            item.Clear();
        }
        roadsPath.Clear();

        foreach (var item in borderPathRight)
        {
            foreach (var listItem in item)
            {
                if (listItem != null)
                {
                    Destroy(listItem.gameObject);
                }
            }
            item.Clear();
        }
        borderPathRight.Clear();

        foreach (var item in cornerPath)
        {
            foreach (var listItem in item)
            {
                if (listItem != null)
                {
                    Destroy(listItem.gameObject);
                }
            }
            item.Clear();
        }
        cornerPath.Clear();

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
            if (item != null)
            {
                Destroy(item);
            }
        }
        dumperBase.Clear();

        roadRenderer.GetComponent<MeshFilter>().mesh.Clear();
        borderRenderer.GetComponent<MeshFilter>().mesh.Clear();
        dustBorderRenderer.GetComponent<MeshFilter>().mesh.Clear();
        dumperRenderer.GetComponent<MeshFilter>().mesh.Clear();
        sideWalkRenderer.GetComponent<MeshFilter>().mesh.Clear();
        centerDumperRenderer.GetComponent<MeshFilter>().mesh.Clear();
    }

    public void ClearDebug()
    {
        if (!drawDebug)
        {
            foreach (var item in debugPoints)
            {
                Destroy(item.gameObject);
            }
        }
        debugPoints.Clear();
    }

    private void Refresh()
    {
        ClearRoad();
        Vector3 currentRotation = this.transform.eulerAngles;
        this.transform.eulerAngles = Vector3.zero;

        RAI_DebugManager.instance.ShowMessage("RefreshRoad " + this.name, Color.green);

        List<Transform> startPointsLeft = new List<Transform>();
        List<Transform> startPointsRight = new List<Transform>();
        List<Transform> endPointsLeft = new List<Transform>();
        List<Transform> endPointsRight = new List<Transform>();

        for (int i = 0; i < roadPoints.Count; i++)
        {

            borderPathLeft.Add(new List<Transform>());
            borderPathRight.Add(new List<Transform>());
            roadsPath.Add(new List<Transform>());

            int width = SettingsManager.GetIntValue(roadPoints[i], settingItemType.road_width);
            roadWidth type = new roadWidth();
            type = (roadWidth)width;
            float wid = SettingsManager.GetRoadWidth(type);

            Transform l_start = new GameObject("l_start").transform;
            Transform l_end = new GameObject("l_end").transform;
            Transform r_start = new GameObject("r_start").transform;
            Transform r_end = new GameObject("r_end").transform;

            r_start.position = roadPoints[i].transform.position + roadPoints[i].transform.right * wid;
            r_start.rotation = roadPoints[i].transform.rotation;
            l_start.position = roadPoints[i].transform.position + roadPoints[i].transform.right * -wid;
            l_start.rotation = roadPoints[i].transform.rotation;

            r_end.position = roadPoints[i].connectedPoint.transform.position + roadPoints[i].connectedPoint.transform.right * wid;
            r_end.rotation = roadPoints[i].connectedPoint.transform.rotation;
            l_end.position = roadPoints[i].connectedPoint.transform.position + roadPoints[i].connectedPoint.transform.right * -wid;
            l_end.rotation = roadPoints[i].connectedPoint.transform.rotation;

            startPointsLeft.Add(l_start);
            startPointsRight.Add(r_start);

            endPointsLeft.Add(l_end);
            endPointsRight.Add(r_end);

            List<Transform> left_border = null;
            List<Transform> right_border = null;

            if (true)
            {
                right_border = (getConnnectedPoints(r_start, r_end, true));
            }
            else
            {
                right_border = (getConnectedPointsStraight(r_start, r_end, true, true));
            }
            if (true)
            {
                left_border = getConnnectedPoints(l_start, l_end, true);
            }
            else
            {
                left_border = (getConnectedPointsStraight(l_start, l_end, true, true));
            }

            foreach (var border_item in left_border)
            {
                border_item.eulerAngles += new Vector3(0, 180, 0); ;
            }

            borderPathRight[i].AddRange(inverseList(right_border));
            borderPathLeft[i].AddRange(left_border);

            roadsPath[i].AddRange(getConnnectedPoints(roadPoints[i].transform, roadPoints[i].connectedPoint.transform, true));

            foreach (var item in roadsPath[i])
            {
                item.eulerAngles += new Vector3(0, 90, 0);
            }

            foreach (var item in right_border)
            {
                Destroy(item.gameObject);
            }
            foreach (var item in left_border)
            {
                Destroy(item.gameObject);
            }
        }

        for (int i = 0; i < startPointsRight.Count; i++)
        {
            cornerPath.Add(new List<Transform>());

            if (i == (startPointsRight.Count - 1))
            {
                cornerPath[i].AddRange(getCornerPoints(startPointsRight[i], startPointsLeft[0]));
            }
            else
            {
                cornerPath[i].AddRange(getCornerPoints(startPointsRight[i], startPointsLeft[i + 1]));
            }
        }

        SortPoints();

        drawVectors = new List<Vector2>();

        foreach (var listItem in bordersPath)
        {
            foreach (var item in listItem)
            {
                item.eulerAngles += new Vector3(0, 90, 0);

                Vector2 point = new Vector2(item.position.x - this.transform.position.x, item.position.z - this.transform.position.z);
                drawVectors.Add(point);

                DrawDebugPoint(item, Color.yellow);
            }
        }

        drawVectors.Add(drawVectors[0]);
        List<Vector3> vert = null;
        List<int> tri = null;
        RAI_Triangulation.Triangulate(drawVectors, out tri, out vert);
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


        foreach (var item in startPointsLeft)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in startPointsRight)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in endPointsLeft)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in endPointsRight)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in cornerPath)
        {
            foreach (var listItem in item)
            {
                if (listItem != null)
                {
                    Destroy(listItem.gameObject);
                }
            }
        }

        this.transform.eulerAngles = currentRotation;
    }

    private List<Transform> getCornerPoints(Transform a, Transform b)
    {
        List<Transform> result = new List<Transform>();
        List<Transform> borderA = new List<Transform>();
        List<Transform> borderB = new List<Transform>();

        float cornerDistance = Vector3.Distance(a.position, b.position);

        Transform looker = new GameObject().transform;
        looker.position = a.position;
        looker.LookAt(b);    

        looker.position = b.position;
        looker.LookAt(a);

        Transform pointA = new GameObject().transform;
        pointA.position = a.position;

        Transform pointB = new GameObject().transform;
        pointB.position = b.position;

        pointA.rotation = a.rotation;
        pointB.eulerAngles = b.rotation.eulerAngles + new Vector3(0, 180, 0);

        if (cornerDistance > cornerGap)
        {
            Intersections intersections = new Intersections();

            Point pCross = new Point();
            Info info;

            Point p1 = new Point(pointA.position.x, pointA.position.z);
            Point p2 = new Point(pointA.position.x + pointA.forward.x, pointA.position.z + pointA.forward.z);
            Point p3 = new Point(pointB.position.x, pointB.position.z);
            Point p4 = new Point(pointB.position.x + pointB.forward.x, pointB.position.z + pointB.forward.z);



            intersections.LineLine(p1, p2, p3, p4, out pCross, out info);

            if (info.Id > 45 || info.Id == 0)
            {
                Vector3 triangleTop = new Vector3((float)pCross.X, a.position.y, (float)pCross.Y);
                looker.position = triangleTop;
                looker.LookAt(a);
                Vector3 angleA = looker.forward;
                looker.LookAt(b);
                Vector3 angleB = looker.forward;

                float degree = Vector3.SignedAngle(angleA, angleB, Vector3.up);
                float gapDistance = cornerGap / 2 * (Mathf.Abs(Mathf.Sin(degree / 2)));
                looker.LookAt(a);
                pointA.position = looker.position + looker.forward * gapDistance;
                looker.LookAt(b);
                pointB.position = looker.position + looker.forward * gapDistance;
                pointB.eulerAngles += new Vector3(0, 180, 0);

                if (roundedCorners)
                {
                    borderA = getConnnectedPoints(pointA, a, false);
                    borderB = getConnnectedPoints(pointB, b, false);
                }
                else
                {
                    borderA = getConnectedPointsStraight(pointA, a, false, true);
                    borderB = getConnectedPointsStraight(pointB, b, false, true);
                }

                foreach (var item in borderB)
                {
                    item.eulerAngles += new Vector3(0, 180, 0);
                }

                DrawDebugPoint(triangleTop, Color.red);
            }
            else
            {
                if (Vector3.Distance(a.position, b.position) > cornerGap)
                {
                    float t = Vector3.Distance(a.position, b.position) / 2;
                    t -= cornerGap / 2;
                    Debug.Log(info.Message);

                    pointA.position = a.position + a.forward * -t;
                    pointB.position = b.position + b.forward * -t;
                    pointB.eulerAngles += new Vector3(0, 180, 0);

                    if (roundedCorners)
                    {
                        borderA = getConnnectedPoints(pointA, a, false);
                        borderB = getConnnectedPoints(pointB, b, false);
                    }
                    else
                    {
                        borderA = getConnectedPointsStraight(pointA, a, false, true);
                        borderB = getConnectedPointsStraight(pointB, b, false, true);
                    }

                    foreach (var item in borderB)
                    {
                        item.eulerAngles += new Vector3(0, 180, 0);
                    }
                }

                else
                {

                }
            }
        }

        pointA.rotation = a.rotation;
        pointB.eulerAngles = b.rotation.eulerAngles + new Vector3(0, 180, 0);

        result.AddRange(inverseList(borderA));

        if (roundedCorners)
        {
            result.AddRange(inverseList(getConnnectedPoints(pointB, pointA, true, true, true)));
        }
        else
        {
            result.AddRange(inverseList(getConnectedPointsStraight(pointB, pointA, true, true)));
        }
        result.AddRange(borderB);

        foreach (var item in borderA)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in borderB)
        {
            Destroy(item.gameObject);
        }

        Destroy(looker.gameObject);
        Destroy(pointA.gameObject);
        Destroy(pointB.gameObject);

        return result;
    }

    public void DrawDebugPoint(Transform pos, Color color)
    {
        if (!drawDebug) return;

        GameObject debugPoint = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debugPoint.transform.position = pos.position;
        debugPoint.transform.rotation = pos.rotation;
        debugPoint.transform.eulerAngles += new Vector3(90, 0 ,0);

        debugPoint.name = pos.name;

        GameObject debugArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugArrow.transform.parent = debugPoint.transform;
        debugArrow.transform.localPosition = new Vector3(0, 1.2f, 0);
        debugArrow.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        debugArrow.transform.localScale = new Vector3(0.2f, 0.2f, 2);
        debugArrow.gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(color, Color.white, 0.5f);
        debugArrow.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.GetComponent<MeshRenderer>().material.color = color;
        debugPoint.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.transform.localScale *= debugSize;
        debugPoint.gameObject.layer = 9;
        debugPoint.transform.parent = GameObject.Find("DebugPoints").transform;

        debugPoints.Add(debugPoint.transform);
    }

    public void DrawDebugPoint(Transform pos, Color color, string name)
    {
        if (!drawDebug) return;

        GameObject debugPoint = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debugPoint.transform.position = pos.position;
        debugPoint.transform.rotation = pos.rotation;
        debugPoint.transform.eulerAngles += new Vector3(90, 0, 0);

        debugPoint.name = name;

        GameObject debugArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugArrow.transform.parent = debugPoint.transform;
        debugArrow.transform.localPosition = new Vector3(0, 1.2f, 0);
        debugArrow.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        debugArrow.transform.localScale = new Vector3(0.2f, 0.2f, 2);
        debugArrow.gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(color, Color.white, 0.5f);
        debugArrow.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.GetComponent<MeshRenderer>().material.color = color;
        debugPoint.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.transform.localScale *= debugSize;
        debugPoint.gameObject.layer = 9;
        debugPoint.transform.parent = GameObject.Find("DebugPoints").transform;

        debugPoints.Add(debugPoint.transform);
    }

    public void DrawDebugPoint(Vector3 pos, Color color)
    {
        if (!drawDebug) return;

        GameObject debugPoint = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debugPoint.transform.position = pos;
        debugPoint.transform.eulerAngles += new Vector3(90, 0, 0);

        debugPoint.GetComponent<MeshRenderer>().material.color = color;
        debugPoint.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.transform.localScale *= debugSize;
        debugPoint.gameObject.layer = 9;
        debugPoint.transform.parent = GameObject.Find("DebugPoints").transform;

        debugPoints.Add(debugPoint.transform);
    }

    public void DrawDebugPoint(Vector3 pos, Color color, string name)
    {
        if (!drawDebug) return;

        GameObject debugPoint = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debugPoint.transform.position = pos;
        debugPoint.transform.eulerAngles += new Vector3(90, 0, 0);
        debugPoint.name = name;

        debugPoint.GetComponent<MeshRenderer>().material.color = color;
        debugPoint.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        debugPoint.transform.localScale *= debugSize;
        debugPoint.gameObject.layer = 9;
        debugPoint.transform.parent = GameObject.Find("DebugPoints").transform;

        debugPoints.Add(debugPoint.transform);
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

        List<Transform> result = getBorderPoints(pointsAmount, distance, a, b, addLastPoints, drawDebug, addFirstPoints, false);

        return result;
    }

    private List<Transform> getConnnectedPoints(Transform a, Transform b, bool addLastPoints, bool addFirstPoints, bool setRotation)
    {
        float distance = Vector3.Distance(a.position, b.position);
        int pointsAmount = Mathf.RoundToInt(distance / pointsFrequency);

        List<Transform> result = getBorderPoints(pointsAmount, distance, a, b, addLastPoints, drawDebug, addFirstPoints, setRotation);

        return result;
    }

    private List<Transform> getBorderPoints(int pointsAmount, float distance, Transform a, Transform b, bool addLastPoints, bool drawDebug, bool addFirstPoints, bool setRotation)
    {
        List<Transform> result = new List<Transform>();

        Vector3 target = a.position;

        Vector3 Aleft = a.position + a.right * (sidewalkWidth + borderDepth);
        Vector3 Bleft = b.position + b.right * (sidewalkWidth + borderDepth);

        if (addLastPoints)
        {
            Transform startPoint = new GameObject().transform;
            startPoint.position = Vector3.Lerp(a.position, b.position, 0);
            startPoint.rotation = a.rotation;
            startPoint.gameObject.name = "startPoint";
            result.Add(startPoint);
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

        return result;
    }

    private List<Transform> getConnectedPointsStraight(Transform a, Transform b, bool addLastPoints, bool addFirstPoints)
    {
        float distance = Vector3.Distance(a.position, b.position);
        int pointsAmount = Mathf.RoundToInt(distance / pointsFrequency);

        List<Transform> result = new List<Transform>();

        Vector3 target = a.position;

        Vector3 Aleft = a.position + a.right * (sidewalkWidth + borderDepth);
        Vector3 Bleft = b.position + b.right * (sidewalkWidth + borderDepth);

        if (addLastPoints)
        {
            Transform startPoint = new GameObject().transform;
            startPoint.position = Vector3.Lerp(a.position, b.position, 0);
            startPoint.rotation = a.rotation;
            startPoint.gameObject.name = "startPoint";
            result.Add(startPoint);
        }

        for (int i = 1; i < pointsAmount; i++)
        {
            float t = (float)i / (float)pointsAmount;

            Transform point = new GameObject().transform;
            point.position = Vector3.Lerp(a.position, b.position, t);
            point.gameObject.name = "point[" + i + "]";

            point.eulerAngles += new Vector3(0, 180, 0);

            result.Add(point);
            target = point.position;
        }

        if (addFirstPoints)
        {
            Transform endPoint = new GameObject().transform;
            endPoint.position = Vector3.Lerp(a.position, b.position, 1);
            endPoint.rotation = b.rotation;
            endPoint.gameObject.name = "endPoint";
            result.Add(endPoint);
        }

        for (int i = 0; i < result.Count; i++)
        {
            if (i < result.Count - 1)
            {
                result[i].LookAt(result[i + 1]);
            }
        }

        return result;
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

    private void SortPoints()
    {
        bordersPath.Clear();

        for (int i = 0; i < roadPoints.Count; i++)
        {
            bordersPath.Add(new List<Transform>());

            if (i != (roadPoints.Count - 1))
            {
                bordersPath[i].AddRange(borderPathRight[i]);
                bordersPath[i].AddRange(cornerPath[i]);
                bordersPath[i].AddRange(borderPathLeft[i + 1]);
            }
            else
            {
                bordersPath[i].AddRange(borderPathRight[i]);
                bordersPath[i].AddRange(cornerPath[i]);
                bordersPath[i].AddRange(borderPathLeft[0]);
            }
        }
    }

    private void AddBorder()
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

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        borderRenderer.GetComponent<MeshFilter>().mesh = mesh;
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

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        centerDumperRenderer.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AddSidewalk()
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

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        dustBorderRenderer.GetComponent<MeshFilter>().mesh = mesh;
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
}

class Intersections
{
    private Point Cross(double a1, double b1, double c1, double a2, double b2, double c2)
    {
        /*
         x = (b1c2 - c1b2) / (a1b2 - a2b1)
         y = (a2c1 - c2a1) / (a1b2 - a2b1)
        */

        Point pCross = new Point(0, 0); ;

        pCross.X = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);

        pCross.Y = (a2 * c1 - a1 * c2) / (a1 * b2 - a2 * b1);


        return pCross;
    }

    public bool LineLine(Point pABDot1, Point pABDot2, Point pCDDot1, Point pCDDot2, out Point pCross, out Info info)
    {
        info = new Info();

        pCross = new Point(0, 0);

        double a1 = pABDot2.Y - pABDot1.Y;
        double b1 = pABDot1.X - pABDot2.X;
        double c1 = -pABDot1.X * pABDot2.Y + pABDot1.Y * pABDot2.X;


        double a2 = pCDDot2.Y - pCDDot1.Y;
        double b2 = pCDDot1.X - pCDDot2.X;
        double c2 = -pCDDot1.X * pCDDot2.Y + pCDDot1.Y * pCDDot2.X;

        pCross = Cross(a1, b1, c1, a2, b2, c2);

        // Обе прямые неопределенны
        if (a1 == 0 && b1 == 0 && a2 == 0 && b2 == 0)
        {
            info.Id = 10;
            info.Message = "Обе прямые не определены";

            return false;
        }


        // Направление первой прямой неопределенно
        if (a1 == 0 && b1 == 0)
        {
            info.Id = 11;
            info.Message = "Первая прямая не определена";

            return false;
        }


        // Направление второй прямой неопределенно
        if (a2 == 0 && b2 == 0)
        {
            info.Id = 12;
            info.Message = "Вторая прямая не определена";

            return false;
        }



        // Прямые параллельны
        if ((a1 * b2 - a2 * b1) == 0)
        {

            info.Id = 40;
            info.Message = "Прямые параллельны";

            if (a1 == 0)
            {
                // Прямые паралельны оси Х
                info.Id = 41;
                info.Message = "Прямые паралельны оси Х";
            }

            if (b1 == 0)
            {
                // Прямые паралелльны оси Y
                info.Id = 42;
                info.Message = "Прямые паралельны оси Y";
            }


            if (a1 * b2 == b1 * a2 && a1 * c2 == a2 * c1 && b1 * c2 == c1 * b2)
            {
                info.Id = 43;
                info.Message = "Прямые совпадают";
            }


            return false;
        }

        pCross = Cross(a1, b1, c1, a2, b2, c2);


        if ((a1 * a2 + b1 * b2) == 0)
        {
            info.Id = 50;
            info.Message = "Прямые перпендикулярны";

            return true;
        }

        if (a1 == 0)
        {
            info.Id = 60;
            info.Message = "Первая прямая параллельна оси Х";

            return true;
        }

        if (a2 == 0)
        {
            info.Id = 61;
            info.Message = "Вторая прямая параллельна оси Х";

            return true;
        }


        // Первая прямая параллельна оси Y
        if (b1 == 0)
        {
            info.Id = 70;
            info.Message = "Первая прямая параллельна оси Y";

            return true;
        }

        // Вторая прямая параллельна оси Y
        if (b2 == 0)
        {

            info.Id = 71;
            info.Message = "Вторая прямая параллельна оси Y";

            return true;
        }


        info.Id = 0;
        info.Message = "Общий случай";


        return true;
    }

}

public class Info
{
    public string Message;
    public int Id;
}

public class Point
{
    public double X;
    public double Y;

    public Point(float x, float y)
    {
        X = (double)x;
        Y = (double)y;
    }
    public Point()
    {
        X = 0;
        Y = 0;
    }

    public override string ToString()
    {
        return "x = " + X + ", y = " + Y;
    }
}


