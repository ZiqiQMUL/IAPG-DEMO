using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField]
    private CellularAutomataCaveGenerator caveGenerator;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
    }

    void CreateMesh()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < caveGenerator.width; x++)
        {
            for (int y = 0; y < caveGenerator.height; y++)
            {
                if (caveGenerator.map[x, y] == 0) // Only create mesh for ground tiles
                {
                    int vertexIndex = vertices.Count;

                    vertices.Add(new Vector3(x, y, 0));
                    vertices.Add(new Vector3(x + 1, y, 0));
                    vertices.Add(new Vector3(x + 1, y + 1, 0));
                    vertices.Add(new Vector3(x, y + 1, 0));

                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 3);
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}