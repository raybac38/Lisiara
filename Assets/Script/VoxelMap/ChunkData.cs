using UnityEngine;

public class ChunkData
{
    private const float perlinNoiseMapScale = 0.015f;
    private const float perlinNoiseStrenght = 25f;
    private const float mapWaterLevel = 50f;
    static private Vector3Int chunkSize = new(32, 32, 32);
    private readonly Vector3Int chunkPosition;
    private readonly Voxel[,,] chunkVoxels = new Voxel[chunkSize.x, chunkSize.y, chunkSize.z];

    public ChunkData(Vector3Int chunkPosition)
    {
        this.chunkPosition = chunkPosition;
    }

    private Vector3Int RelativeToAbsolutCoordinate(Vector3Int relative)
    {
        return relative + Vector3Int.Scale(chunkPosition, chunkSize);
    }

    public void Generate()
    {
        Vector3Int startingPosition = RelativeToAbsolutCoordinate(Vector3Int.zero);
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int z = 0; z < chunkSize.z; z++)
            {
                float groundLevel = Mathf.PerlinNoise(perlinNoiseMapScale * (x + startingPosition.x), perlinNoiseMapScale * (z + startingPosition.z));
                groundLevel = groundLevel * groundLevel * perlinNoiseStrenght + mapWaterLevel;
                int maxY = Mathf.Min(chunkSize.y, Mathf.FloorToInt(groundLevel - startingPosition.y));
                for (int y = 0; y < maxY; y++)
                {
                    chunkVoxels[x, y, z].type = VoxelType.Stone;
                }
            }
        }
    }
}
