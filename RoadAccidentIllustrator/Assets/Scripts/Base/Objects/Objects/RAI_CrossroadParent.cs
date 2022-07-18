using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAI_CrossroadParent : PDDERoad
{
    public List<RAI_CrossroadCenter> points;
    public MeshCollider meshCollider;

    public override void Start()
    {
        base.Start();

        foreach (var item in points)
        {
            item.parent = this;
        }

        meshCollider = this.GetComponent<MeshCollider>();

        //UpdateMesh();

        StartCoroutine(StartRefresh());

    }

    public override void ApplySettings()
    {
        base.ApplySettings();

        foreach (var item in points)
        {
            item.ClearRoad();
        }

        StartCoroutine(StartRefresh());

        //UpdateMesh();
    }

    public override void OnMovedEnd()
    {
        base.OnMoved();

        foreach (var item in points)
        {
            item.ClearRoad();
        }

        StartCoroutine(StartRefresh());

        //UpdateMesh();
    }

    public override void OnRotatedEnd()
    {
        base.OnRotated();

        foreach (var item in points)
        {
            item.ClearRoad();
        }

        StartCoroutine(StartRefresh());

        //UpdateMesh();
    }

    public override void OnMoved()
    {
        base.OnMoved();
        foreach (var item in points)
        {
            //item.ClearRoad();
        }
    }

    public override void OnRotated()
    {
        base.OnRotated();
        foreach (var item in points)
        {
            //item.ClearRoad();
        }
    }

    private void UpdateMesh()
    {
        List<Mesh> meshes = new List<Mesh>();

        foreach (var item in points)
        {
            if (item.GetComponent<MeshCollider>())
            {
                meshes.Add(item.GetComponent<MeshCollider>().sharedMesh);
            }
        }

        Debug.Log(meshes.Count);

        int vertOffset = 0;
        int triOffset = 0;

        List<Vector3> vert = new List<Vector3>();
        List<int> tri = new List<int>();

        foreach (var item in meshes)
        {
            for (int i = 0; i < item.vertices.Length; i++)
            {
                vert.Add(item.vertices[i]);
            }
            for (int i = 0; i < item.triangles.Length; i++)
            {
                tri.Add(item.triangles[i] + vertOffset);
            }

            vertOffset += item.vertices.Length;
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vert.ToArray();
        mesh.triangles = tri.ToArray();

        meshCollider.sharedMesh = mesh;
    }

    IEnumerator StartRefresh()
    {
        yield return new WaitForEndOfFrame();

        foreach (var item in points)
        {
            item.objectSettings = this.objectSettings;

            yield return new WaitForEndOfFrame();

            item.RefreshRoad();
        }

        yield return new WaitForEndOfFrame();

        UpdateMesh();
    }
}
