using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class MapMeshBuilder
{
    private readonly Vector3Int mapSize;
    private readonly bool[,,] map;

    public MapMeshBuilder(Vector3Int mapSize, bool[,,] map)
    {
        this.mapSize = mapSize;
        this.map = map;
    }


    private static readonly Vector3[] faceNormals = new Vector3[]
    {
    Vector3.forward,  // Z+
    Vector3.back,     // Z-
    Vector3.up,       // Y+
    Vector3.down,     // Y-
    Vector3.right,    // X+
    Vector3.left      // X-
    };
    private static readonly Vector3Int[] verticalDirection = new Vector3Int[]
    {
        new (0,1,0),
        new (0,-1,0)
    };




    private static readonly Vector3Int[] directions = new Vector3Int[]
    {
    new (0, 0, 1),   // Z+
    new (0, 0, -1),  // Z-
    new (0, 1, 0),   // Y+
    new (0, -1, 0),  // Y-
    new (1, 0, 0),   // X+
    new (-1, 0, 0),  // X-
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


    private bool IsInsideMap(Vector3Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.z >= 0 &&
               pos.x < mapSize.x && pos.y < mapSize.y && pos.z < mapSize.z;
    }

    public Mesh GeneratingGreedyMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        for (int y = 0; y < mapSize.y; y++)
        {
            bool[,] planeMap = new bool[mapSize.x, mapSize.z];
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Vector3Int current = new Vector3Int(x, y, z);
                    Vector3Int above = current + Vector3Int.up;
                    if (map[x, y, z] && (!IsInsideMap(above) || !map[above.x, above.y, above.z]))
                    {
                        planeMap[x, z] = true;
                    }
                }
            }


            for (int x = 0; x < mapSize.x; x++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    if (!planeMap[x, z]) continue;

                    int width = 1;
                    int height = 1;

                    // Étendre en X
                    while (x + width < mapSize.x && planeMap[x + width, z])
                    {
                        width++;
                    }

                    // Étendre en Z (seulement si toute la ligne en X est valide)
                    bool done = false;
                    while (z + height < mapSize.z && !done)
                    {
                        for (int dx = 0; dx < width; dx++)
                        {
                            if (!planeMap[x + dx, z + height])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) height++;
                    }

                    // Nettoyer la zone utilisée
                    for (int dx = 0; dx < width; dx++)
                    {
                        for (int dz = 0; dz < height; dz++)
                        {
                            planeMap[x + dx, z + dz] = false;
                        }
                    }

                    // Création des vertices du quad
                    int vertIndex = vertices.Count;

                    vertices.Add(new Vector3(x, y + 1, z));                       // Bottom Left
                    vertices.Add(new Vector3(x, y + 1, z + height));             // Top Left
                    vertices.Add(new Vector3(x + width, y + 1, z + height));     // Top Right
                    vertices.Add(new Vector3(x + width, y + 1, z));              // Bottom Right

                    // Deux triangles
                    triangles.Add(vertIndex);
                    triangles.Add(vertIndex + 1);
                    triangles.Add(vertIndex + 2);

                    triangles.Add(vertIndex);
                    triangles.Add(vertIndex + 2);
                    triangles.Add(vertIndex + 3);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }


    public Mesh GenerateMesh()
    {
        Profiler.BeginSample("Generating mesh");
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



        Mesh mesh = new();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();


        Profiler.EndSample();

        return mesh;
    }
}
