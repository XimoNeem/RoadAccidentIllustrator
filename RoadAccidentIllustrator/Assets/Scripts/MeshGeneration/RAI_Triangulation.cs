using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TriangleNet.Geometry;

public static class RAI_Triangulation
{
    public static bool Triangulate(List<Vector2> points, out List<int> outIndexes, out List<Vector3> outVertices)
    {
        outVertices = new List<Vector3>();
        outIndexes = new List<int>();

        Polygon polygon = new Polygon();

        for (int i = 0; i < points.Count; i++)
        {
            polygon.Add(new Vertex(points[i].x, points[i].y));

            if (i == (points.Count - 1))
            {
                polygon.Add(new Segment(new Vertex(points[i].x, points[i].y), new Vertex(points[0].x, points[0].y)));
            }
            else 
            {
                polygon.Add(new Segment(new Vertex(points[i].x, points[i].y), new Vertex(points[i + 1].x, points[i + 1].y)));
            }
        }

        var mesh = polygon.Triangulate();

        foreach (ITriangle item in mesh.Triangles)
        {
            for (int j = 2; j >= 0 ; j--)
            {
                bool found = false;

                for (int k = 0; k < outVertices.Count ; k++)
                {
                    if ((outVertices[k].x == item.GetVertex(j).X) && (outVertices[k].z == item.GetVertex(j).Y))
                    {
                        outIndexes.Add(k);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    outVertices.Add(new Vector3((float)item.GetVertex(j).X, 0, (float)item.GetVertex(j).Y));
                    outIndexes.Add(outVertices.Count - 1);
                }
            }
        }

        return true;
    }
}
