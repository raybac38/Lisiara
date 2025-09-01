using System;
using UnityEngine;

public class MapData
{
    static public readonly Vector3Int chunkSize = new(32, 32, 32);
    static public readonly Vector3Int mapSize = new(8, 8, 8);
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
        int chunkX = x / chunkSize.x;
        int chunkY = y / chunkSize.y;
        int chunkZ = z / chunkSize.z;

        Vector3Int chunkCoordinate = new(chunkX, chunkY, chunkZ);

        if (IsOutside(chunkCoordinate))
        {
            return new Voxel { type = VoxelType.Air };
        }

        int localX = ((x % chunkSize.x) + chunkSize.x) % chunkSize.x;
        int localY = ((y % chunkSize.y) + chunkSize.y) % chunkSize.y;
        int localZ = ((z % chunkSize.z) + chunkSize.z) % chunkSize.z;

        return loadedChunks[chunkX, chunkY, chunkZ]
            .GetLocalVoxel(localX, localY, localZ);
    }


    private bool IsOutside(Vector3Int chunkCoord)
    {
        return chunkCoord.x < 0 || chunkCoord.y < 0 || chunkCoord.z < 0 ||
               chunkCoord.x >= mapSize.x || chunkCoord.y >= mapSize.y || chunkCoord.z >= mapSize.z;
    }

}
