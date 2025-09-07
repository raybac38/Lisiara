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

    private bool IsFaceVisible(int posX, int posY, int posZ, int normalX, int normalY, int normalZ)
    {
        int absX = posX + chunkPosition.x * MapData.CHUNK_SIZE_X;
        int absY = posY + chunkPosition.y * MapData.CHUNK_SIZE_Y;
        int absZ = posZ + chunkPosition.z * MapData.CHUNK_SIZE_Z;

        return mapData.GetVoxelData(absX, absY, absZ).type != VoxelType.Air &&
            mapData.GetVoxelData(absX + normalX, absY + normalY, absZ + normalZ).type == VoxelType.Air;
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

    private void GenerateTopFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        int xmax = MapData.CHUNK_SIZE_X;
        int ymax = MapData.CHUNK_SIZE_Y;
        int zmax = MapData.CHUNK_SIZE_Z;


        bool[,] mask = new bool[xmax, zmax];
        for (int y = 0; y < ymax; y++)
        {
            bool containVisibleFace = false;
            for (int x = 0; x < xmax; x++)
            {
                for (int z = 0; z < zmax; z++)
                {
                    bool isVisible = IsFaceVisible(x, y, z, 0, 1, 0);   /// UP
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

    private void GenerateBottomFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        int xmax = MapData.CHUNK_SIZE_X;
        int ymax = MapData.CHUNK_SIZE_Y;
        int zmax = MapData.CHUNK_SIZE_Z;


        bool[,] mask = new bool[xmax, zmax];
        for (int y = 0; y < ymax; y++)
        {
            bool containVisibleFace = false;
            for (int x = 0; x < xmax; x++)
            {
                for (int z = 0; z < zmax; z++)
                {
                    bool isVisible = IsFaceVisible(x, y, z, 0, -1, 0);   /// DOWN
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

                    Vector3 bottomLeft = new Vector3(startingx, y, startingz);
                    Vector3 bottomRight = new Vector3(endingx, y, startingz);
                    Vector3 topLeft = new Vector3(startingx, y, endingz);
                    Vector3 topRight = new Vector3(endingx, y, endingz);

                    int index = vertices.Count;

                    vertices.Add(bottomLeft);
                    vertices.Add(topLeft);
                    vertices.Add(topRight);
                    vertices.Add(bottomRight);

                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);

                    triangles.Add(index);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);

                    uvs.Add(new Vector2(startingx, startingz));
                    uvs.Add(new Vector2(endingx, startingz));
                    uvs.Add(new Vector2(startingx, endingz));
                    uvs.Add(new Vector2(endingx, endingz));
                }
            }
        }
    }

    private void GenerateFrontFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        int xmax = MapData.CHUNK_SIZE_X;
        int ymax = MapData.CHUNK_SIZE_Y;
        int zmax = MapData.CHUNK_SIZE_Z;


        bool[,] mask = new bool[xmax, ymax];
        for (int z = 0; z < zmax; z++)
        {
            bool containVisibleFace = false;
            for (int x = 0; x < xmax; x++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    bool isVisible = IsFaceVisible(x, y, z, 0, 0, 1);   /// FORWARD
                    mask[x, y] = isVisible;
                    containVisibleFace |= isVisible;
                }
            }

            if (!containVisibleFace) continue;

            for (int x = 0; x < xmax; x++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    if (!mask[x, y]) continue;
                    int startingx = x;
                    int startingy = y;

                    int endingx = x;
                    int endingy = y;

                    while (endingx < xmax && mask[endingx, endingy])
                    {
                        endingx++;
                    }


                    bool done = false;
                    while (!done)
                    {
                        int newY = endingy + 1;
                        if (newY >= ymax) break;

                        for (int innerx = startingx; innerx < endingx; innerx++)
                        {
                            if (!mask[innerx, newY])
                            {
                                done = true;
                                break;
                            }
                        }

                        if (!done)
                        {
                            endingy++;
                        }
                    }
                    endingy++;

                    for (int innerx = startingx; innerx < endingx; innerx++)
                    {
                        for (int innery = startingy; innery < endingy; innery++)
                        {
                            mask[innerx, innery] = false;
                        }
                    }

                    Vector3 bottomLeft = new Vector3(startingx, startingy, z + 1);
                    Vector3 bottomRight = new Vector3(endingx, startingy, z + 1);
                    Vector3 topLeft = new Vector3(startingx, endingy, z + 1);
                    Vector3 topRight = new Vector3(endingx, endingy, z + 1);

                    int index = vertices.Count;

                    vertices.Add(bottomLeft);
                    vertices.Add(topLeft);
                    vertices.Add(topRight);
                    vertices.Add(bottomRight);

                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);

                    triangles.Add(index);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);

                    uvs.Add(new Vector2(startingx, startingy));
                    uvs.Add(new Vector2(endingx, startingy));
                    uvs.Add(new Vector2(startingx, endingy));
                    uvs.Add(new Vector2(endingx, endingy));
                }
            }
        }
    }

    private void GenerateRightFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        int xmax = MapData.CHUNK_SIZE_X;
        int ymax = MapData.CHUNK_SIZE_Y;
        int zmax = MapData.CHUNK_SIZE_Z;

        bool[,] mask = new bool[zmax, ymax];
        for (int x = 0; x < xmax; x++)
        {
            bool containVisibleFace = false;
            for (int z = 0; z < zmax; z++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    bool isVisible = IsFaceVisible(x, y, z, 1, 0, 0);   /// RIGHT
                    mask[z, y] = isVisible;
                    containVisibleFace |= isVisible;
                }
            }

            if (!containVisibleFace) continue;

            for (int z = 0; z < zmax; z++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    if (!mask[z, y]) continue;
                    int startingz = z;
                    int startingy = y;

                    int endingz = z;
                    int endingy = y;

                    while (endingz < zmax && mask[endingz, endingy])
                    {
                        endingz++;
                    }


                    bool done = false;
                    while (!done)
                    {
                        int newY = endingy + 1;
                        if (newY >= ymax) break;

                        for (int innerz = startingz; innerz < endingz; innerz++)
                        {
                            if (!mask[innerz, newY])
                            {
                                done = true;
                                break;
                            }
                        }

                        if (!done)
                        {
                            endingy++;
                        }
                    }
                    endingy++;

                    for (int innerz = startingz; innerz < endingz; innerz++)
                    {
                        for (int innery = startingy; innery < endingy; innery++)
                        {
                            mask[innerz, innery] = false;
                        }
                    }

                    Vector3 bottomLeft = new Vector3(x + 1, startingy, startingz);
                    Vector3 bottomRight = new Vector3(x + 1, startingy, endingz);
                    Vector3 topLeft = new Vector3(x + 1, endingy, startingz);
                    Vector3 topRight = new Vector3(x + 1, endingy, endingz);

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

                    uvs.Add(new Vector2(startingz, startingy));
                    uvs.Add(new Vector2(endingz, startingy));
                    uvs.Add(new Vector2(startingz, endingy));
                    uvs.Add(new Vector2(endingz, endingy));
                }
            }
        }
    }



    private void GenerateBackFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        int xmax = MapData.CHUNK_SIZE_X;
        int ymax = MapData.CHUNK_SIZE_Y;
        int zmax = MapData.CHUNK_SIZE_Z;


        bool[,] mask = new bool[xmax, ymax];
        for (int z = 0; z < zmax; z++)
        {
            bool containVisibleFace = false;
            for (int x = 0; x < xmax; x++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    bool isVisible = IsFaceVisible(x, y, z, 0, 0, -1);   /// BACKWARD
                    mask[x, y] = isVisible;
                    containVisibleFace |= isVisible;
                }
            }

            if (!containVisibleFace) continue;

            for (int x = 0; x < xmax; x++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    if (!mask[x, y]) continue;
                    int startingx = x;
                    int startingy = y;

                    int endingx = x;
                    int endingy = y;

                    while (endingx < xmax && mask[endingx, endingy])
                    {
                        endingx++;
                    }


                    bool done = false;
                    while (!done)
                    {
                        int newY = endingy + 1;
                        if (newY >= ymax) break;

                        for (int innerx = startingx; innerx < endingx; innerx++)
                        {
                            if (!mask[innerx, newY])
                            {
                                done = true;
                                break;
                            }
                        }

                        if (!done)
                        {
                            endingy++;
                        }
                    }
                    endingy++;

                    for (int innerx = startingx; innerx < endingx; innerx++)
                    {
                        for (int innery = startingy; innery < endingy; innery++)
                        {
                            mask[innerx, innery] = false;
                        }
                    }

                    Vector3 bottomLeft = new Vector3(startingx, startingy, z);
                    Vector3 bottomRight = new Vector3(endingx, startingy, z);
                    Vector3 topLeft = new Vector3(startingx, endingy, z);
                    Vector3 topRight = new Vector3(endingx, endingy, z);

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

                    uvs.Add(new Vector2(startingx, startingy));
                    uvs.Add(new Vector2(endingx, startingy));
                    uvs.Add(new Vector2(startingx, endingy));
                    uvs.Add(new Vector2(endingx, endingy));
                }
            }
        }
    }

    private void GenerateLeftFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        int xmax = MapData.CHUNK_SIZE_X;
        int ymax = MapData.CHUNK_SIZE_Y;
        int zmax = MapData.CHUNK_SIZE_Z;

        bool[,] mask = new bool[zmax, ymax];
        for (int x = 0; x < xmax; x++)
        {
            bool containVisibleFace = false;
            for (int z = 0; z < zmax; z++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    bool isVisible = IsFaceVisible(x, y, z, -1, 0, 0);   /// LEFT
                    mask[z, y] = isVisible;
                    containVisibleFace |= isVisible;
                }
            }

            if (!containVisibleFace) continue;

            for (int z = 0; z < zmax; z++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    if (!mask[z, y]) continue;
                    int startingz = z;
                    int startingy = y;

                    int endingz = z;
                    int endingy = y;

                    while (endingz < zmax && mask[endingz, endingy])
                    {
                        endingz++;
                    }


                    bool done = false;
                    while (!done)
                    {
                        int newY = endingy + 1;
                        if (newY >= ymax) break;

                        for (int innerz = startingz; innerz < endingz; innerz++)
                        {
                            if (!mask[innerz, newY])
                            {
                                done = true;
                                break;
                            }
                        }

                        if (!done)
                        {
                            endingy++;
                        }
                    }
                    endingy++;

                    for (int innerz = startingz; innerz < endingz; innerz++)
                    {
                        for (int innery = startingy; innery < endingy; innery++)
                        {
                            mask[innerz, innery] = false;
                        }
                    }

                    Vector3 bottomLeft = new Vector3(x , startingy, startingz);
                    Vector3 bottomRight = new Vector3(x , startingy, endingz);
                    Vector3 topLeft = new Vector3(x , endingy, startingz);
                    Vector3 topRight = new Vector3(x , endingy, endingz);

                    int index = vertices.Count;

                    vertices.Add(bottomLeft);
                    vertices.Add(topLeft);
                    vertices.Add(topRight);
                    vertices.Add(bottomRight);

                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);

                    triangles.Add(index);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);

                    uvs.Add(new Vector2(startingz, startingy));
                    uvs.Add(new Vector2(endingz, startingy));
                    uvs.Add(new Vector2(startingz, endingy));
                    uvs.Add(new Vector2(endingz, endingy));
                }
            }
        }
    }



    public void GenerateMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        /// Y
        GenerateTopFace(vertices, triangles, uvs);
        GenerateBottomFace(vertices, triangles, uvs);
        /// Z
        GenerateFrontFace(vertices, triangles, uvs);
        GenerateBackFace(vertices, triangles, uvs);
        /// X
        GenerateRightFace(vertices, triangles, uvs);
        GenerateLeftFace(vertices, triangles, uvs);


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
