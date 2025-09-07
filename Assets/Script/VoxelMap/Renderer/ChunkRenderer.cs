using System;
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

    private delegate bool FaceVisibility(int x, int y, int z);
    private delegate void AddFaceVertices(List<Vector3> vertices, int startA, int startB, int endA, int endB, int fixedCoord);

    private void GenerateFace(
        int sizeA, int sizeB, int sizeFixed,
        FaceVisibility isVisible,
        AddFaceVertices addVertices,
        in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs,
        bool invertTriangles = false)
    {
        bool[,] mask = new bool[sizeA, sizeB];
        for (int fixedCoord = 0; fixedCoord < sizeFixed; fixedCoord++)
        {
            bool containVisibleFace = false;
            for (int a = 0; a < sizeA; a++)
            {
                for (int b = 0; b < sizeB; b++)
                {
                    bool visible = isVisible(a, b, fixedCoord);
                    mask[a, b] = visible;
                    containVisibleFace |= visible;
                }
            }
            if (!containVisibleFace) continue;

            for (int a = 0; a < sizeA; a++)
            {
                for (int b = 0; b < sizeB; b++)
                {
                    if (!mask[a, b]) continue;
                    int startA = a, startB = b, endA = a, endB = b;

                    while (endA < sizeA && mask[endA, endB]) endA++;

                    bool done = false;
                    while (!done)
                    {
                        int newB = endB + 1;
                        if (newB >= sizeB) break;
                        for (int innerA = startA; innerA < endA; innerA++)
                        {
                            if (!mask[innerA, newB])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endB++;
                    }
                    endB++;

                    for (int innerA = startA; innerA < endA; innerA++)
                        for (int innerB = startB; innerB < endB; innerB++)
                            mask[innerA, innerB] = false;

                    int index = vertices.Count;
                    addVertices(vertices, startA, startB, endA, endB, fixedCoord);

                    if (!invertTriangles)
                    {
                        triangles.Add(index);
                        triangles.Add(index + 1);
                        triangles.Add(index + 2);
                        triangles.Add(index);
                        triangles.Add(index + 2);
                        triangles.Add(index + 3);
                    }
                    else
                    {
                        triangles.Add(index);
                        triangles.Add(index + 2);
                        triangles.Add(index + 1);
                        triangles.Add(index);
                        triangles.Add(index + 3);
                        triangles.Add(index + 2);
                    }

                    uvs.Add(new Vector2(startA, startB));
                    uvs.Add(new Vector2(endA, startB));
                    uvs.Add(new Vector2(startA, endB));
                    uvs.Add(new Vector2(endA, endB));
                }
            }
        }
    }

    // Exemples d’utilisation pour chaque face :
    private void GenerateTopFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        GenerateFace(
            MapData.CHUNK_SIZE_X, MapData.CHUNK_SIZE_Z, MapData.CHUNK_SIZE_Y,
            (x, z, y) => IsFaceVisible(x, y, z, 0, 1, 0),
            (verts, sx, sz, ex, ez, y) =>
            {
                verts.Add(new Vector3(sx, y + 1, sz));  // Bas gauche
                verts.Add(new Vector3(sx, y + 1, ez));  // Haut gauche
                verts.Add(new Vector3(ex, y + 1, ez));  // Haut droite
                verts.Add(new Vector3(ex, y + 1, sz));  // Bas droite
            },
            vertices, triangles, uvs, false
        );
    }

    private void GenerateBottomFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        GenerateFace(
            MapData.CHUNK_SIZE_X, MapData.CHUNK_SIZE_Z, MapData.CHUNK_SIZE_Y,
            (x, z, y) => IsFaceVisible(x, y, z, 0, -1, 0),
            (verts, sx, sz, ex, ez, y) =>
            {
                verts.Add(new Vector3(sx, y, sz));    // Bas gauche
                verts.Add(new Vector3(ex, y, sz));    // Bas droite
                verts.Add(new Vector3(ex, y, ez));    // Haut droite
                verts.Add(new Vector3(sx, y, ez));    // Haut gauche
            },
            vertices, triangles, uvs, false
        );
    }

    private void GenerateFrontFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        GenerateFace(
            MapData.CHUNK_SIZE_X, MapData.CHUNK_SIZE_Y, MapData.CHUNK_SIZE_Z,
            (x, y, z) => IsFaceVisible(x, y, z, 0, 0, 1),
            (verts, sx, sy, ex, ey, z) =>
            {
                verts.Add(new Vector3(sx, sy, z + 1));    // Bas gauche
                verts.Add(new Vector3(ex, sy, z + 1));    // Bas droite
                verts.Add(new Vector3(ex, ey, z + 1));    // Haut droite
                verts.Add(new Vector3(sx, ey, z + 1));    // Haut gauche
            },
            vertices, triangles, uvs, false
        );
    }

    private void GenerateBackFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        GenerateFace(
            MapData.CHUNK_SIZE_X, MapData.CHUNK_SIZE_Y, MapData.CHUNK_SIZE_Z,
            (x, y, z) => IsFaceVisible(x, y, z, 0, 0, -1),
            (verts, sx, sy, ex, ey, z) =>
            {
                verts.Add(new Vector3(ex, sy, z));    // Bas droite
                verts.Add(new Vector3(sx, sy, z));    // Bas gauche
                verts.Add(new Vector3(sx, ey, z));    // Haut gauche
                verts.Add(new Vector3(ex, ey, z));    // Haut droite
            },
            vertices, triangles, uvs, false
        );
    }

    private void GenerateRightFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        GenerateFace(
            MapData.CHUNK_SIZE_Z, MapData.CHUNK_SIZE_Y, MapData.CHUNK_SIZE_X,
            (z, y, x) => IsFaceVisible(x, y, z, 1, 0, 0),
            (verts, sz, sy, ez, ey, x) =>
            {
                verts.Add(new Vector3(x + 1, sy, ez));    // Bas droite
                verts.Add(new Vector3(x + 1, sy, sz));    // Bas gauche
                verts.Add(new Vector3(x + 1, ey, sz));    // Haut gauche
                verts.Add(new Vector3(x + 1, ey, ez));    // Haut droite
            },
            vertices, triangles, uvs, false
        );
    }

    private void GenerateLeftFace(in List<Vector3> vertices, in List<int> triangles, in List<Vector2> uvs)
    {
        GenerateFace(
            MapData.CHUNK_SIZE_Z, MapData.CHUNK_SIZE_Y, MapData.CHUNK_SIZE_X,
            (z, y, x) => IsFaceVisible(x, y, z, -1, 0, 0),
            (verts, sz, sy, ez, ey, x) =>
            {
                verts.Add(new Vector3(x, sy, sz));    // Bas gauche
                verts.Add(new Vector3(x, sy, ez));    // Bas droite
                verts.Add(new Vector3(x, ey, ez));    // Haut droite
                verts.Add(new Vector3(x, ey, sz));    // Haut gauche
            },
            vertices, triangles, uvs, false
        );
    }

    public void GenerateMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        // Génération de toutes les faces
        GenerateTopFace(vertices, triangles, uvs);
        GenerateBottomFace(vertices, triangles, uvs);
        GenerateFrontFace(vertices, triangles, uvs);
        GenerateBackFace(vertices, triangles, uvs);
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
