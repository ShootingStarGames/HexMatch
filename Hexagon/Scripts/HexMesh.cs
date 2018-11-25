using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        Triangulate();

        if(GetComponent<MeshCollider>())
            GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void Triangulate()
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        Calculate();

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void Calculate()
    {
        Vector3 center = Vector3.zero;
        Vector2 uvCenter = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );
            AddUV(
                uvCenter,
                uvCenter + HexMetrics.uvCorners[i],
                uvCenter + HexMetrics.uvCorners[i + 1]
                );
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddUV(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        uvs.Add(v1);
        uvs.Add(v2);
        uvs.Add(v3);
    }
}