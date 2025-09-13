using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class ChunkRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer renderer;

    public Vector3Int chunkPosition;

    public MapData mapData;

    private VoxelMeshJobHandle jobHandle;

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


    private void Start()
    {
        ChunkData chunkData = mapData.GetChunkData(chunkPosition.x, chunkPosition.y, chunkPosition.z);
        jobHandle = new VoxelMeshJobHandle(chunkData);
        jobHandle.GenerateMesh();
    }

    private void Update()
    {
        if (jobHandle != null)
        {
            if (jobHandle.IsMeshGenerated())
            {
                mesh = jobHandle.GetMesh();
                jobHandle = null;
            }
        }

        meshFilter .mesh = mesh;
        renderer.material = MapRenderer.defaultStaticMaterial;

    }

}
