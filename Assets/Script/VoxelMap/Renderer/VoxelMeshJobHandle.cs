using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class VoxelMeshJobHandle
{
    private NativeList<float3> vertices = new(Allocator.TempJob);
    private NativeList<int> triangles = new(Allocator.TempJob);
    private NativeList<float2> uvs = new(Allocator.TempJob);

    private readonly ChunkData chunkData;

    private JobHandle jobHandle;

    private readonly Mesh mesh = new ();

    public VoxelMeshJobHandle(ChunkData chunkData)
    {
        this.chunkData = chunkData;
    }

    public void GenerateMesh()
    {
        VoxelMeshJob job = new()
        {
            voxels = chunkData.voxels,
            vertices = vertices,
            triangles = triangles,
            uvs = uvs
        };
        jobHandle = job.Schedule();
    }


    public bool IsMeshGenerated()
    {
        return jobHandle.IsCompleted;
    }

    public Mesh GetMesh()
    {
        jobHandle.Complete();
        List<Vector3> verts = new(vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
            verts.Add(vertices[i]);

        List<int> tris = new(triangles.Length);
        for (int i = 0; i < triangles.Length; i++)
            tris.Add(triangles[i]);

        List<Vector2> uvList = new(uvs.Length);
        for (int i = 0; i < uvs.Length; i++)
            uvList.Add(uvs[i]);


        mesh.Clear();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvList);
        mesh.RecalculateNormals();

        vertices.Dispose();
        triangles.Dispose();
        uvs.Dispose();

        return mesh;
    }
    
}
