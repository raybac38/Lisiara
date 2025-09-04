using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChunkRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer renderer;

    public Vector3Int chunkPosition;

    public MapData mapData;

    private bool IsFaceVisible(Vector3Int relativePosition, Vector3Int faceNormal)
    {
        Vector3Int absolutPosition = relativePosition + (chunkPosition * MapData.chunkSize);
        return mapData.GetVoxelData(absolutPosition.x, absolutPosition.y, absolutPosition.z).type != VoxelType.Air &&
            mapData.GetVoxelData(absolutPosition.x + faceNormal.x, absolutPosition.y + faceNormal.y, absolutPosition.z + faceNormal.z).type == VoxelType.Air;
    }

    private void Awake()
    {
        mesh = new();
        mesh.name = gameObject.name;
        renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
    }

    private void GenerateTopFace(in List<Vector3> vertices,in List<int> triangles,in List<Vector2> uvs)
    {
        int xmax = MapData.chunkSize.x;
        int ymax = MapData.chunkSize.y;
        int zmax = MapData.chunkSize.z;

        bool[,] mask = new bool[xmax, zmax];
        for (int y = 0; y < ymax; y++)
        {
            bool containVisibleFace = false;
            for (int x = 0; x < xmax; x++)
            {
                for (int z = 0; z < zmax; z++)
                {
                    bool isVisible = IsFaceVisible(new Vector3Int(x, y, z), Vector3Int.up);
                    mask[x, z] = isVisible;
                    containVisibleFace |= isVisible;
                }
            }

            if (!containVisibleFace) continue;

            for (int x = 0; x < xmax; x++)
            {
                for (int z = 0; z < zmax; z++)
                {
                    if (!mask[x, z]) continue;
                    int startingx = x;
                    int startingz = z;

                    int endingx = x;
                    int endingz = z;

                    while (endingx < xmax && mask[endingx, endingz])
                    {
                        endingx++;
                    }


                    bool done = false;
                    while (!done)
                    {
                        int newZ = endingz + 1;
                        if (newZ >= zmax) break;

                        for (int innerx = startingx; innerx < endingx; innerx++)
                        {
                            if (!mask[innerx, newZ])
                            {
                                done = true;
                                break;
                            }
                        }

                        if (!done)
                        {
                            endingz++;
                        }
                    }
                    endingz++;

                    for (int innerx = startingx; innerx < endingx; innerx++)
                    {
                        for (int innerz = startingz; innerz < endingz; innerz++)
                        {
                            mask[innerx, innerz] = false;
                        }
                    }

                    Vector3 bottomLeft = new Vector3(startingx, y + 1, startingz);
                    Vector3 bottomRight = new Vector3(endingx, y + 1, startingz);
                    Vector3 topLeft = new Vector3(startingx, y + 1, endingz);
                    Vector3 topRight = new Vector3(endingx, y + 1, endingz);

                    int index = vertices.Count;

                    vertices.Add(bottomLeft);
                    vertices.Add(topLeft);
                    vertices.Add(topRight);
                    vertices.Add(bottomRight);

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);

                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new Vector2(startingx, startingz));
                    uvs.Add(new Vector2(endingx, startingz));
                    uvs.Add(new Vector2(startingx, endingz));
                    uvs.Add(new Vector2(endingx, endingz));
                }
            }
        }
    }


    public void GenerateMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();


        GenerateTopFace(vertices, triangles, uvs);          

        mesh.Clear();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        renderer.material = MapRenderer.defaultStaticMaterial;
    }



}
