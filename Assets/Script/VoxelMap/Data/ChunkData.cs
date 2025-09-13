using Unity.Collections;
using UnityEngine;

public class ChunkData
{
    private const float perlinNoiseMapScale = 0.015f;
    private const float perlinNoiseStrenght = 25f;
    private const float mapWaterLevel = 50f;
    private readonly Vector3Int chunkPosition;

    public NativeArray<Voxel> voxels = new(MapData.CHUNK_SIZE_X * MapData.CHUNK_SIZE_Y * MapData.CHUNK_SIZE_Z, Allocator.Persistent);

    private Voxel Get(int x, int y, int z)
    {
        return voxels[x + MapData.CHUNK_SIZE_X * (y + MapData.CHUNK_SIZE_Y * z)];

    }
    private void Set(int x, int y, int z, Voxel voxel)
    {
        voxels[x + MapData.CHUNK_SIZE_X * (y + MapData.CHUNK_SIZE_Y * z)] = voxel;
    }

    public ChunkData(Vector3Int chunkPosition)
    {
        this.chunkPosition = chunkPosition;
    }

    private Vector3Int RelativeToAbsolutCoordinate(Vector3Int relative)
    {
        return relative + Vector3Int.Scale(chunkPosition, new Vector3Int(MapData.CHUNK_SIZE_X, MapData.CHUNK_SIZE_Y, MapData.CHUNK_SIZE_Z));
    }

    public void Generate()
    {
        Vector3Int startingPosition = RelativeToAbsolutCoordinate(Vector3Int.zero);
        for (int x = 0; x < MapData.CHUNK_SIZE_X; x++)
        {
            for (int z = 0; z < MapData.CHUNK_SIZE_Z; z++)
            {
                float groundLevel = Mathf.PerlinNoise(perlinNoiseMapScale * (x + startingPosition.x), perlinNoiseMapScale * (z + startingPosition.z));
                groundLevel = groundLevel * groundLevel * perlinNoiseStrenght + mapWaterLevel;
                int maxY = Mathf.Min(MapData.CHUNK_SIZE_Y, Mathf.FloorToInt(groundLevel - startingPosition.y));
                for (int y = 0; y < maxY; y++)
                {
                    Set(x,y,z, new Voxel {type = VoxelType.Stone});
                }
            }
        }
    }

    public Voxel GetLocalVoxel(int x, int y, int z) {

        return Get(x, y, z);
    }
}
