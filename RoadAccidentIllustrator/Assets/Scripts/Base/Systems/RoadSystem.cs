using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSystem : MonoBehaviour
{
    public List<PDDERoadPoint> points;
    public float roadWidth;
    public Material mat;
    public LineRenderer lineRenderer;

    public List<Vector3> vertices;

    GameObject road;

    private void OnEnable()
    {
        EventBus.OnObjectMove += CheckMovement;
    }

    private void OnDisable()
    {
        EventBus.OnObjectMove -= CheckMovement;
    }
    private void Start()
    {
        vertices = new List<Vector3>();

        lineRenderer.startWidth = roadWidth;
        lineRenderer.endWidth = roadWidth;
    }

    void CheckMovement()
    {
        if (ObjectMover.instance.selectedObjects[0].GetComponent<PDDERoadPoint>())
        {
            RefreshRoad();
        }
    }

    public void RefreshRoad()
    {
        Destroy(road);
        vertices.Clear();
        RAI_DebugManager.instance.ShowMessage("Refresh road system");

        foreach (var item in points)
        {
            vertices.Add(item.transform.position);
        }
        lineRenderer.positionCount = vertices.Count;
        lineRenderer.SetPositions(vertices.ToArray());

        /*
        for (int i = 0; i < vertices.Count; i++)
        {
            lineRenderer.SetPosition(i, vertices[i]);
        }


        /*
        road = new GameObject("roadSystem");
        road.transform.position = Vector3.zero;
        MeshRenderer meshRenderer = road.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = road.AddComponent<MeshFilter>();
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertices);
        newMesh.UploadMeshData(false);

        meshFilter.mesh = newMesh;
        meshRenderer.material = mat;
        */
    }
}
