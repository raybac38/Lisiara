using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour 
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer renderer;

    public Vector3Int chunkPosition;

    public MapData mapData;


    private readonly Vector3Int[] Direction = {
        Vector3Int.up, Vector3Int.down,
        Vector3Int.left, Vector3Int.right,
        Vector3Int.forward, Vector3Int.back,
    };

    private (Vector3Int first, Vector3Int second) GetPerpendicular(Vector3Int direction)
    {
        if (direction == Vector3Int.up || direction == Vector3Int.down)
        {
            return (Vector3Int.left, Vector3Int.forward);
        }
        if (direction == Vector3Int.left || direction == Vector3Int.right)
        {
            return (Vector3Int.up, Vector3Int.forward);
        }
        if (direction == Vector3Int.forward || direction == Vector3Int.back)
        {
            return (Vector3Int.up, Vector3Int.left);
        }
        throw new System.Exception("direction should be one of the six cardinal coordinate");
    }

    private bool IsFaceVisible(Vector3Int relativePosition, Vector3Int faceNormal)
    {
        Vector3Int absolutPosition = relativePosition + (chunkPosition * MapData.chunkSize);
        return mapData.GetVoxelData(absolutPosition.x, absolutPosition.y, absolutPosition.z).type != VoxelType.Air &&
            mapData.GetVoxelData(absolutPosition.x + faceNormal.x, absolutPosition.y + faceNormal.y, absolutPosition.z + faceNormal.z).type == VoxelType.Air;
    }

    private void Awake()
    {
        mesh = new Mesh();
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

    public void GenerateMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        int vertexIndex = 0;

        foreach (Vector3Int faceNormal in Direction)
        {
            Vector3Int vec01, vec10;
            (vec01, vec10) = GetPerpendicular(faceNormal);

            for (int x = 0; x < MapData.chunkSize.x; x++)
            {
                for (int y = 0; y < MapData.chunkSize.y; y++)
                {
                    for (int z = 0; z < MapData.chunkSize.z; z++)
                    {
                        Vector3Int localPos = new(x, y, z);

                        if (IsFaceVisible(localPos, faceNormal))
                        {
                            Vector3Int worldPos = localPos + chunkPosition * MapData.chunkSize;

                            Vector3Int bottomLeft = worldPos;
                            Vector3Int bottomRight = worldPos + vec10;
                            Vector3Int topLeft = worldPos + vec01;
                            Vector3Int topRight = worldPos + vec01 + vec10;

                            vertices.Add(bottomLeft);
                            vertices.Add(bottomRight);
                            vertices.Add(topRight);
                            vertices.Add(topLeft);

                            triangles.Add(vertexIndex + 0);
                            triangles.Add(vertexIndex + 1);
                            triangles.Add(vertexIndex + 2);

                            triangles.Add(vertexIndex + 2);
                            triangles.Add(vertexIndex + 3);
                            triangles.Add(vertexIndex + 0);

                            uvs.Add(new Vector2(0, 0));
                            uvs.Add(new Vector2(1, 0));
                            uvs.Add(new Vector2(1, 1));
                            uvs.Add(new Vector2(0, 1));

                            vertexIndex += 4;
                        }
                    }
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

}
