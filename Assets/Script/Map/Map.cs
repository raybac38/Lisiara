using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Map : MonoBehaviour
{
    [SerializeField]
    static private Vector3Int mapSize = new Vector3Int(1024, 125, 1024);
    [SerializeField]
    private bool[,,] map = new bool[mapSize.x, mapSize.y, mapSize.z];
    [SerializeField]
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    [SerializeField]
    private Material material;
    [SerializeField]
    private GameObject settler;

    public Pathfiding Pathfiding { get; } = new Pathfiding();

    public int GetSurfaceY(Vector2Int coordinate)
    {
        coordinate.x = ((coordinate.x % mapSize.x) + mapSize.x) % mapSize.x;
        coordinate.y = ((coordinate.y % mapSize.z) + mapSize.z) % mapSize.z;
        for (int y = mapSize.y - 1; y > 1; y--) { 
            if (map[coordinate.x, y - 1 , coordinate.y])
            {
                return y;
            }
        }
        return -1;
    }

    private void Start()
    {
        Profiler.BeginSample("MapBuilding");
        MapBuilder mapBuilder = new();
        map = mapBuilder.GenerateProceduralMap(mapSize);
        Profiler.EndSample();

        Profiler.BeginSample("MeshBuilding");
        MapMeshBuilder mapMeshBuilder = new(mapSize, map);
        mesh = mapMeshBuilder.GeneratingGreedyMesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        Profiler.EndSample();

        Profiler.BeginSample("MaterialSetting");
        meshRenderer.material = material;
        Profiler.EndSample();

        Profiler.BeginSample("PathfindingSetup");
        Pathfiding.GeneratePathMap(map);
        Profiler.EndSample();
    }
}
