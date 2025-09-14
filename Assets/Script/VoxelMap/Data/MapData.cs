using System;
using Unity.Burst;
using UnityEngine;

public class MapData
{
    /// <summary>
    /// CHUNK SIZE NEED TO BE A POWER OF 2 FOR EFFICIENCY PURPOSE
    /// </summary>

    public const int CHUNK_SIZE_X = 32;
    public const int CHUNK_SIZE_Y = 32;
    public const int CHUNK_SIZE_Z = 32;

    private const int CHUNK_SHIFT_X = 5; 
    private const int CHUNK_SHIFT_Y = 5;
    private const int CHUNK_SHIFT_Z = 5;

    private const int CHUNK_MASK_X = CHUNK_SIZE_X - 1; 
    private const int CHUNK_MASK_Y = CHUNK_SIZE_Y - 1;
    private const int CHUNK_MASK_Z = CHUNK_SIZE_Z - 1;


    //static public readonly Vector3Int mapSize = new(32, 4, 32);

    public const int MAP_SIZE_X = 32;
    public const int MAP_SIZE_Y = 4;
    public const int MAP_SIZE_Z = 32;

    private readonly ChunkData[,,] loadedChunks;

    public MapData()
    {
        loadedChunks = new ChunkData[MAP_SIZE_X, MAP_SIZE_Y, MAP_SIZE_Z];
        for (int x = 0; x < MAP_SIZE_X; x++)
        {
            for (int y = 0; y < MAP_SIZE_Y; y++)
            {
                for (int z = 0; z < MAP_SIZE_Z; z++)
                {
                    ChunkData chunkData = new(new(x, y, z));
                    loadedChunks[x, y, z] = chunkData;
                    chunkData.Generate();
                }
            }
        }
    }

    public Voxel GetVoxelData(int x, int y, int z)
    {
        int chunkX = x >> CHUNK_SHIFT_X;
        int chunkY = y >> CHUNK_SHIFT_Y;
        int chunkZ = z >> CHUNK_SHIFT_Z;

        if (IsOutside(chunkX,chunkY,chunkZ))
        {
            return new Voxel { type = VoxelType.Air };
        }

        int localX = x & CHUNK_MASK_X;
        int localY = y & CHUNK_MASK_Y;
        int localZ = z & CHUNK_MASK_Z;

        return loadedChunks[chunkX, chunkY, chunkZ]
            .GetLocalVoxel(localX, localY, localZ);
    }


    private bool IsOutside(int x, int y, int z)
    {
        return x < 0 || y < 0 || z < 0 ||
               x >= MAP_SIZE_X || y >= MAP_SIZE_Y || z >= MAP_SIZE_Z;
    }

    public ChunkData GetChunkData(int x, int y, int z) { return loadedChunks[x, y, z]; }

    public void Dispose()
    {
        foreach (ChunkData chunk in loadedChunks)
        {
            chunk.Dispose();
        }
    }

}
