using System;
using UnityEngine;

public class MapData
{
    /// <summary>
    /// CHUNK SIZE NEED TO BE A POWER OF 2 FOR EFFICIENCY PURPOSE
    /// </summary>
    static public readonly Vector3Int chunkSize = new(32, 32, 32);

    private const int CHUNK_SIZE_X = 32; 
    private const int CHUNK_SIZE_Y = 32;
    private const int CHUNK_SIZE_Z = 32;

    private const int CHUNK_SHIFT_X = 5; 
    private const int CHUNK_SHIFT_Y = 5;
    private const int CHUNK_SHIFT_Z = 5;

    private const int CHUNK_MASK_X = CHUNK_SIZE_X - 1; 
    private const int CHUNK_MASK_Y = CHUNK_SIZE_Y - 1;
    private const int CHUNK_MASK_Z = CHUNK_SIZE_Z - 1;


    static public readonly Vector3Int mapSize = new(32, 4, 32);
    //static public readonly Vector3Int mapSize = new(4, 4, 4);
    private ChunkData[,,] loadedChunks;

    public MapData()
    {
        loadedChunks = new ChunkData[mapSize.x, mapSize.y, mapSize.z];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    ChunkData chunkData = new(new(x, y, z));
                    loadedChunks[x, y, z] = chunkData;
                    chunkData.Generate();
                    Console.WriteLine("chargement d'un chunk");
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
               x >= mapSize.x || y >= mapSize.y || z >= mapSize.z;
    }

}
