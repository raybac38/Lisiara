using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class BoxelMap : MonoBehaviour
{
    [SerializeField]
    static private Vector3Int mapSize = new Vector3Int(255, 125, 255);
    [SerializeField]
    private bool[,,] map = new bool[mapSize.x, mapSize.y, mapSize.z];
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private Material material;

    private static readonly Vector3[] faceNormals = new Vector3[]
    {
    Vector3.forward,  // Z+
    Vector3.back,     // Z-
    Vector3.up,       // Y+
    Vector3.down,     // Y-
    Vector3.right,    // X+
    Vector3.left      // X-
    };

    private static readonly Vector3Int[] directions = new Vector3Int[]
    {
    new Vector3Int(0, 0, 1),   // Z+
    new Vector3Int(0, 0, -1),  // Z-
    new Vector3Int(0, 1, 0),   // Y+
    new Vector3Int(0, -1, 0),  // Y-
    new Vector3Int(1, 0, 0),   // X+
    new Vector3Int(-1, 0, 0),  // X-
    };

    private static readonly Vector3[][] faceVertices = new Vector3[][]
    {
    new Vector3[] { new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(0,1,1) }, // Z+
    new Vector3[] { new Vector3(1,0,0), new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,1,0) }, // Z-
    new Vector3[] { new Vector3(0,1,1), new Vector3(1,1,1), new Vector3(1,1,0), new Vector3(0,1,0) }, // Y+
    new Vector3[] { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(0,0,1) }, // Y-
    new Vector3[] { new Vector3(1,0,1), new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(1,1,1) }, // X+
    new Vector3[] { new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(0,1,0) }  // X-
    };

    private static readonly Vector2[] faceUVs = new Vector2[]
    {
    new Vector2(0, 0),
    new Vector2(1, 0),
    new Vector2(1, 1),
    new Vector2(0, 1)
    };


    /// <summary>
    /// Generate the mesh from the map
    /// </summary>
    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int faceCount = 0;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    if (!map[x, y, z]) continue;

                    Vector3 cubePos = new Vector3(x, y, z);

                    for (int i = 0; i < 6; i++)
                    {
                        Vector3Int dir = directions[i];
                        Vector3Int neighbor = new Vector3Int(x, y, z) + dir;

                        if (!IsInsideMap(neighbor) || !map[neighbor.x, neighbor.y, neighbor.z])
                        {
                            int vStart = vertices.Count;

                            foreach (Vector3 vert in faceVertices[i])
                            {
                                vertices.Add(cubePos + vert);
                                normals.Add(faceNormals[i]);
                                uvs.Add(faceUVs[Array.IndexOf(faceVertices[i], vert)]);
                            }

                            triangles.Add(vStart);
                            triangles.Add(vStart + 1);
                            triangles.Add(vStart + 2);
                            triangles.Add(vStart);
                            triangles.Add(vStart + 2);
                            triangles.Add(vStart + 3);

                            faceCount++;
                        }
                    }
                }
            }
        }

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private bool IsInsideMap(Vector3Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.z >= 0 &&
               pos.x < mapSize.x && pos.y < mapSize.y && pos.z < mapSize.z;
    }



    void Start()
    {
        MapBuilder mapBuilder = new MapBuilder();
        this.map = mapBuilder.Generate(mapSize);
        GenerateMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
