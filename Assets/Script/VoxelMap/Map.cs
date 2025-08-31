using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap
{
    private readonly Vector3Int mapSize = new(16, 8, 16);
    private ChunkData[,,] loadedChunks;

    void Awake()
    {
        loadedChunks = new ChunkData[mapSize.x,mapSize.y,mapSize.z];
        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    ChunkData chunkData = new ChunkData(new(x, y, z));
                    loadedChunks[x, y, z] = chunkData;
                    Console.WriteLine("chargement d'un chunk");

                }
            }
        }
    }
}
